using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace ChaosRecipeEnhancer.UI.Common;

/// <summary>
/// This class ensures that only one instance of an application runs and provides a mechanism
/// to notify an already running instance of subsequent attempts to start the application.
/// </summary>
public sealed class SingleInstance : IDisposable
{
    // Mutex is used to ensure that only one instance of the process is running
    private readonly Mutex _instanceMutex;
    private readonly string _instanceName; // Name of the Mutex
    private readonly bool _listenForOtherInstances; // Flag to listen for other instances
    private NamedPipeServerStream _server; // For communication with other instances

    /// <summary>
    /// Constructs a SingleInstance object with a specific instance name.
    /// </summary>
    /// <param name="instanceName">The unique name for this instance used for the mutex.</param>
    /// <param name="listenForOtherInstances">Indicates whether to listen for other instances or not.</param>
    public SingleInstance(string instanceName, bool listenForOtherInstances = true)
    {
        _instanceMutex = new Mutex(true, instanceName);
        _instanceName = instanceName;
        _listenForOtherInstances = listenForOtherInstances;
    }

    /// <summary>
    /// Disposes the mutex and named pipe server.
    /// </summary>
    public void Dispose()
    {
        _instanceMutex.Dispose();
        _server?.Dispose();
    }

    /// <summary>
    /// Occurs when the single instance application is pinged by another process.
    /// </summary>
    public event EventHandler PingedByOtherProcess;

    /// <summary>
    /// Attempts to claim the single instance ownership.
    /// </summary>
    /// <returns>true if the instance is claimed successfully; otherwise, false.</returns>
    public bool Claim()
    {
        // Try to claim the mutex
        if (!_instanceMutex.WaitOne(TimeSpan.Zero))
        {
            return false; // Instance already running
        }

        // If claimed and listening is enabled, start the listener for other instances
        if (_listenForOtherInstances) ListenForOtherProcesses();
        return true;
    }

    /// <summary>
    /// Notifies the already running single instance with some data.
    /// </summary>
    /// <param name="dataToSend">The data to send to the running instance.</param>
    public void PingSingleInstance(string dataToSend)
    {
        // Connect to the named pipe and write data
        using var client = new NamedPipeClientStream(".", _instanceName, PipeDirection.Out);
        try
        {
            client.Connect(0); // Attempt to connect to the server
            using var writer = new StreamWriter(client);
            writer.Write(dataToSend);
            writer.Flush();
        }
        catch
        {
            // Could not connect to the pipe, or another error occurred
        }
    }

    /// <summary>
    /// Listens for other processes attempting to start this application instance.
    /// </summary>
    private void ListenForOtherProcesses()
    {
        _server = new NamedPipeServerStream(
            _instanceName,
            PipeDirection.In,
            1, // Only allow 1 connection
            PipeTransmissionMode.Message,
            PipeOptions.Asynchronous
        );

        // Begin waiting for a connection asynchronously
        _ = _server.BeginWaitForConnection(OnPipeConnection, _server);
    }

    /// <summary>
    /// Callback for when a connection is made to the named pipe server.
    /// </summary>
    /// <param name="ar">The result of the asynchronous operation.</param>
    private void OnPipeConnection(IAsyncResult ar)
    {
        using var server = (NamedPipeServerStream)ar.AsyncState;
        try
        {
            // Complete the connection
            server.EndWaitForConnection(ar);

            // Read data sent from the client
            using var reader = new StreamReader(server);
            var dataReceived = reader.ReadToEnd();

            // Invoke the event to handle the received data
            PingedByOtherProcess?.Invoke(dataReceived, EventArgs.Empty);
        }
        catch
        {
            // An error occurred during the connection
        }

        // Continue listening for the next connection
        ListenForOtherProcesses();
    }
}
