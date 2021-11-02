using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EnhancePoE.Model
{
    public class LogWatcher
    {
        private FileSystemWatcher watcher = new FileSystemWatcher();

        public static Thread WorkerThread { get; set; }
        public static string LastZone { get; set; } = "";
        public static string NewZone { get; set; } = "";

        private static readonly int cooldown = 120;

        private static bool FetchAllowed { get; set; } = true;

        public LogWatcher()
        {

            Trace.WriteLine("logwatcher created");
            //string[] sep2 = { chaosStart };
            //string[] split2 = split[0].Split(sep2, System.StringSplitOptions.None);

            //string[] sep = { @"\Client.txt" };
            //string[] split = Properties.Settings.Default.LogLocation.Split(sep, System.StringSplitOptions.None);
            //watcher.Path = @"""" + split[0] + "";
            //watcher.Path = Path.GetDirectoryName(@"" + Properties.Settings.Default.LogLocation);
            //watcher.NotifyFilter = NotifyFilters.LastWrite;
            //watcher.Filter = "Client.txt";
            //watcher.Changed += new FileSystemEventHandler(OnChanged);
            //watcher.EnableRaisingEvents = true;

            var wh = new AutoResetEvent(false);
            var fsw = new FileSystemWatcher(Path.GetDirectoryName(@"" + Properties.Settings.Default.LogLocation));
            fsw.Filter = "Client.txt";
            fsw.EnableRaisingEvents = true;
            fsw.Changed += (s, e) => wh.Set();

            var fs = new FileStream(Properties.Settings.Default.LogLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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
                            string[] phrase = GetPhraseTranslation();
                            string hideout = GetHideoutTranslation();
                            string harbour = GetHarbourTranslation();
                            if(s.Contains(phrase[0]) && s.Contains(phrase[1]) && !s.Contains(hideout) && !s.Contains(harbour))
                            {
                                LastZone = NewZone;
                                NewZone = s.Substring(s.IndexOf(phrase[0]));
                                //Trace.WriteLine("entered new zone");
                                Trace.WriteLine(NewZone);
                                FetchIfPossible();
                            }
                        }
                            //Console.WriteLine(s);
                            
                        else
                            wh.WaitOne(1000);
                    }
                }
            });
            //WorkerThread.Start();
            StartWatchingLogFile();

            //wh.Close();
        }

        //Ihr habt 'Sonnenspitze-Versteck' betreten.

        public static string[] GetPhraseTranslation()
        {
            string[] ret = new string[2];
            ret[1] = "";
            switch (Properties.Settings.Default.Language)
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
            switch (Properties.Settings.Default.Language)
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
            switch (Properties.Settings.Default.Language)
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

        public async void FetchIfPossible()
        {
            if (FetchAllowed)
            {
                FetchAllowed = false;
                try
                {
                    MainWindow.overlay.RunFetching();
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

        //private void OnChanged(object source, FileSystemEventArgs e)
        //{
        //    Trace.WriteLine("log file changed");
        //}

    }
}
