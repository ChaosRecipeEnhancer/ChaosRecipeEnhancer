using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace ChaosRecipeEnhancer.UI.Utilities;

public sealed class SingleInstance : IDisposable
{
    private readonly Mutex _instanceMutex;
    private readonly string _instanceName;
    private readonly bool _listenForOtherInstances;
    private NamedPipeServerStream _server;

    public SingleInstance(string instanceName, bool listenForOtherInstances = true)
    {
        _instanceMutex = new Mutex(true, instanceName);
        _instanceName = instanceName;
        _listenForOtherInstances = listenForOtherInstances;
    }

    public void Dispose()
    {
        _instanceMutex.Dispose();
        _server?.Dispose();
    }

    public event EventHandler PingedByOtherProcess;

    public bool Claim()
    {
        if (!_instanceMutex.WaitOne(TimeSpan.Zero))
        {
            return false;
        }

        if (_listenForOtherInstances) ListenForOtherProcesses();
        return true;
    }

    public void PingSingleInstance(string dataToSend)
    {
        // The act of connecting indicates to the single instance that another process tried to run
        using var client = new NamedPipeClientStream(".", _instanceName, PipeDirection.Out);
        try
        {
            client.Connect(0);
            using var writer = new StreamWriter(client);
            writer.Write(dataToSend);
            writer.Flush();
        }
        catch
        {
            // Handle connection failure
        }
    }

    private void ListenForOtherProcesses()
    {
        _server = new NamedPipeServerStream(
            _instanceName,
            PipeDirection.In,
            1,
#pragma warning disable CA1416
            PipeTransmissionMode.Message,
#pragma warning restore CA1416
            PipeOptions.Asynchronous
        );

        _ = _server.BeginWaitForConnection(OnPipeConnection, _server);
    }

    private void OnPipeConnection(IAsyncResult ar)
    {
        using var server = (NamedPipeServerStream)ar.AsyncState;
        try
        {
            server.EndWaitForConnection(ar);
            using var reader = new StreamReader(server);
            var dataReceived = reader.ReadToEnd();

            // Invoke the event with data
            PingedByOtherProcess?.Invoke(dataReceived, EventArgs.Empty);
        }
        catch
        {
            // Handle connection failure
        }

        ListenForOtherProcesses();
    }
}