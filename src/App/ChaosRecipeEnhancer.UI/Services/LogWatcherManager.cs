using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Windows;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ChaosRecipeEnhancer.UI.Services;

public class LogWatcherManager
{
    private static bool AutoFetchAllowed { get; set; } = true;

    // this should match the cooldown we apply in the set tracker view model
    /// <see cref="SetTrackerOverlayViewModel.FetchCooldownSeconds"/>
    private const int AutoFetchCooldownSeconds = 15;
    private static string LastZone { get; set; } = "";
    private static string NewZone { get; set; } = "";

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Task _workerTask;

    public LogWatcherManager(SetTrackerOverlayWindow setTrackerOverlay)
    {
        Log.Information("LogWatcherManager - New Instance Created");
        _cancellationTokenSource = new CancellationTokenSource();

        _workerTask = Dispatcher.CurrentDispatcher.InvokeAsync(() =>
        {
            Task.Factory.StartNew(() => StartWatchingLogFile(setTrackerOverlay));
        }).Task;
    }

    private void StartWatchingLogFile(SetTrackerOverlayWindow setTrackerOverlay)
    {
        Log.Information("LogWatcherManager - Starting Watching Client.txt Log File");

        using var wh = new AutoResetEvent(false);
        using var fsw = new FileSystemWatcher(Path.GetDirectoryName(@"" + Settings.Default.PathOfExileClientLogLocation))
        {
            Filter = "Client.txt",
            EnableRaisingEvents = true
        };

        fsw.Changed += (s, e) => wh.Set();

        using var fs = new FileStream(Settings.Default.PathOfExileClientLogLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        fs.Position = fs.Length;

        using var sr = new StreamReader(fs);

        try
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var s = sr.ReadLine();
                if (s != null)
                {
                    var phrase = GetPhraseTranslation();
                    var hideout = GetHideoutTranslation();
                    var harbour = GetHarbourTranslation();

                    if (s.Contains(phrase[0]) && s.Contains(phrase[1]) && !s.Contains(hideout) && !s.Contains(harbour))
                    {
                        LastZone = NewZone;
                        NewZone = s.Substring(s.IndexOf(phrase[0]));

                        Log.Information("LogWatcherManager - Entered New Zone");
                        Log.Information(NewZone);

                        FetchIfPossible(setTrackerOverlay);
                    }
                }
                else
                {
                    wh.WaitOne(1000);
                }
            }
        }
        catch (ObjectDisposedException ex)
        {
            Log.Error(ex, "LogWatcherManager - Log File Disposed");
        }
    }

    private static void FetchIfPossible(SetTrackerOverlayWindow setTrackerOverlay)
    {
        if (AutoFetchAllowed)
        {
            setTrackerOverlay.RunFetchingAsync();
            TriggerAutoFetchCooldown(AutoFetchCooldownSeconds);
        }
    }

    private static string[] GetPhraseTranslation()
    {
        var ret = new string[2];
        ret[1] = "";
        switch (Settings.Default.Language)
        {
            //english
            case 0:
                ret[0] = "You have entered";
                break;
            //french
            case 1:
                ret[0] = "Vous êtes à présent dans";
                break;
            //german
            case 2:
                ret[0] = "Ihr habt";
                ret[1] = "betreten.";
                break;
            //portuguese
            case 3:
                ret[0] = "Você entrou em";
                break;
            //russian
            case 4:
                ret[0] = "Вы вошли в область";
                break;
            //spanish
            case 5:
                ret[0] = "Has entrado a";
                break;
            //chinese
            case 6:
                ret[0] = "진입했습니다";
                break;
        }

        return ret;
    }

    private static string GetHideoutTranslation()
    {
        switch (Settings.Default.Language)
        {
            case 0:
                return "Hideout";
            case 1:
                return "Repaire";
            case 2:
                return "Versteck";
            case 3:
                return "Refúgio";
            case 4:
                return "Убежище";
            case 5:
                return "Guarida";
            case 6:
                return "은신처";
            default:
                return "";
        }
    }

    private static string GetHarbourTranslation()
    {
        switch (Settings.Default.Language)
        {
            case 0:
                return "The Rogue Harbour";
            case 1:
                return "Le Port des Malfaiteurs";
            case 2:
                return "Der Hafen der Abtrünnigen";
            case 3:
                return "O Porto dos Renegados";
            case 4:
                return "Разбойничья гавань";
            case 5:
                return "El Puerto de los renegados";
            case 6:
                return "도둑 항구에";
            default:
                return "";
        }
    }

    public void StopWatchingLogFile()
    {
        _cancellationTokenSource.Cancel();
        _workerTask.Wait();

        Log.Information("LogWatcherManager - Stopped Watching Client.txt Log File");
    }

    public static void TriggerAutoFetchCooldown(int secondsToWait)
    {
        // Ensure operation on the UI thread if called from another thread
        Application.Current.Dispatcher.Invoke(() =>
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(secondsToWait)
            };

            // define the action to take when the timer ticks
            timer.Tick += (sender, args) =>
            {
                AutoFetchAllowed = true; // Re-enable the auto fetch functionality
                timer.Stop(); // Stop the timer to avoid it triggering again
                Log.Information("LogWatcherManager - Fetch cooldown timer has ended");
            };

            Log.Information("LogWatcherManager - Starting fetch cooldown timer for {SecondsToWait} seconds", secondsToWait);
            AutoFetchAllowed = false; // Disable the fetch button before starting the timer
            timer.Start(); // Start the cooldown timer
        });
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();
    }
}