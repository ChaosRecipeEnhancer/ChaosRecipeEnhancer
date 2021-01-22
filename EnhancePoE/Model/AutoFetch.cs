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

        private int cooldown = 120;

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
                            if(s.Contains("You have entered") && !s.Contains("Hideout"))
                            {
                                LastZone = NewZone;
                                NewZone = s.Substring(s.IndexOf("You have entered"));
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
