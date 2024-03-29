using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Windows;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ChaosRecipeEnhancer.UI.Services;

public class LogWatcherManager
{
    private static bool AutoFetchAllowed { get; set; } = true;
    private const int AutoFetchCooldown = 60;
    private static string LastZone { get; set; } = "";
    private static string NewZone { get; set; } = "";

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Task _workerTask;

    public LogWatcherManager(SetTrackerOverlayWindow setTrackerOverlay)
    {
        Trace.WriteLine("LogWatcherManager created");
        _cancellationTokenSource = new CancellationTokenSource();
        _workerTask = Task.Run(() => StartWatchingLogFile(setTrackerOverlay));
    }

    private void StartWatchingLogFile(SetTrackerOverlayWindow setTrackerOverlay)
    {
        Trace.WriteLine("starting watching");

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

                    Trace.WriteLine("entered new zone");
                    Trace.WriteLine(NewZone);

                    FetchIfPossible(setTrackerOverlay);
                }
            }
            else
            {
                wh.WaitOne(1000);
            }
        }
    }

    private async void FetchIfPossible(SetTrackerOverlayWindow setTrackerOverlay)
    {
        if (AutoFetchAllowed)
        {
            AutoFetchAllowed = false;
            try
            {
                setTrackerOverlay.RunFetchingAsync();

                // enforce cooldown on fetch button to reduce chances of rate limiting
                Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
                {
                    try
                    {
                        await Task.Factory.StartNew(() => Thread.Sleep(AutoFetchCooldown * 1000));
                    }
                    finally
                    {
                        AutoFetchAllowed = true;
                        Trace.WriteLine("allow fetch");
                    }
                });
            }
            catch
            {
                AutoFetchAllowed = true;
            }
        }
    }

    private string[] GetPhraseTranslation()
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

    private string GetHideoutTranslation()
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

    private string GetHarbourTranslation()
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
        Trace.WriteLine("stop watch");
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();
    }
}