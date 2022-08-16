using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.View;

namespace ChaosRecipeEnhancer.UI.Model
{
    public class LogWatcher
    {
        private static readonly int cooldown = 120;
        private FileSystemWatcher watcher = new FileSystemWatcher();

        public LogWatcher(SetTrackerOverlayView setTrackerOverlay)
        {
            Trace.WriteLine("logwatcher created");

            var wh = new AutoResetEvent(false);
            var fsw = new FileSystemWatcher(Path.GetDirectoryName(@"" + Settings.Default.PathOfExileClientLogLocation));
            fsw.Filter = "Client.txt";
            fsw.EnableRaisingEvents = true;
            fsw.Changed += (s, e) => wh.Set();

            var fs = new FileStream(Settings.Default.PathOfExileClientLogLocation, FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite);
            fs.Position = fs.Length;
            WorkerThread = new Thread(() =>
            {
                using (var sr = new StreamReader(fs))
                {
                    var s = "";
                    while (true)
                    {
                        s = sr.ReadLine();
                        if (s != null)
                        {
                            var phrase = GetPhraseTranslation();
                            var hideout = GetHideoutTranslation();
                            var harbour = GetHarbourTranslation();

                            if (s.Contains(phrase[0]) && s.Contains(phrase[1]) && !s.Contains(hideout) &&
                                !s.Contains(harbour))
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
            });
            //WorkerThread.Start();
            StartWatchingLogFile();

            //wh.Close();
        }

        public static Thread WorkerThread { get; set; }
        public static string LastZone { get; set; } = "";
        public static string NewZone { get; set; } = "";

        private static bool FetchAllowed { get; set; } = true;

        //Ihr habt 'Sonnenspitze-Versteck' betreten.

        public static string[] GetPhraseTranslation()
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

        public static string GetHideoutTranslation()
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

        public static string GetHarbourTranslation()
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

        public static void StartWatchingLogFile()
        {
            WorkerThread.Start();
            Trace.WriteLine("starting watching");
        }

        public static void StopWatchingLogFile()
        {
            WorkerThread.Abort();
            Trace.WriteLine("stop watch");
        }

        public async void FetchIfPossible(SetTrackerOverlayView setTrackerOverlay)
        {
            if (FetchAllowed)
            {
                FetchAllowed = false;
                try
                {
                    setTrackerOverlay.RunFetching();
                    await Task.Delay(cooldown * 1000).ContinueWith(_ =>
                    {
                        FetchAllowed = true;
                        Trace.WriteLine("allow fetch");
                    });
                }
                catch
                {
                    FetchAllowed = true;
                }
            }
        }
    }
}