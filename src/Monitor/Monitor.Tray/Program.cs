using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using Common.Logging;
using Monitor.Common;
using Monitor.Schedule;
using Monitor.Communication;
using System.Data;
using Monitor.Schedule.Vestas;
using Monitor.Schedule.GE;

namespace Monitor.Tray
{
    static class Program
    {
        private static ILog logger = LogManager.GetCurrentClassLogger();
        public static NotifyIcon appIcon;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Threading.Mutex appSingleTon = new Mutex(false, "Service Monitor");
            if (appSingleTon.WaitOne(0, false))
            {
                Application.EnableVisualStyles();
                Program.IntializeIcon();
                Microsoft.Win32.SystemEvents.SessionEnded += new
                        Microsoft.Win32.SessionEndedEventHandler(SystemEvent_SessionEnded);



                // Start before Application Run
                appIcon.ContextMenu.MenuItems[0].Enabled = false;
                appIcon.ContextMenu.MenuItems[1].Enabled = true;
                appIcon.Icon = Monitor.Tray.Properties.Resources.ServiceStart;

                Start();

                Application.Run();
            }
            appSingleTon.Close();
        }

        private static void IntializeIcon()
        {
            appIcon = new NotifyIcon();
            appIcon.Icon = Monitor.Tray.Properties.Resources.ServiceStop;
            appIcon.Visible = true;

            ContextMenu mnu = new ContextMenu();

            MenuItem startItem = new MenuItem("Start");
            //startItem.DefaultItem = true;
            startItem.Click += new EventHandler(StartItem_Click);
            mnu.MenuItems.Add(startItem);

            MenuItem stopItem = new MenuItem("Stop");
            //stopItem.DefaultItem = true;
            stopItem.Enabled = false;
            stopItem.Click += new EventHandler(StopItem_Click);
            mnu.MenuItems.Add(stopItem);


            mnu.MenuItems.Add("-");
            MenuItem exitItem = new MenuItem("Exit");
            exitItem.Click += new EventHandler(ExitItem_Click);
            mnu.MenuItems.Add(exitItem);


            appIcon.ContextMenu = mnu;
            appIcon.Text = "Monitor Manager";
        }

        private static void SystemEvent_SessionEnded(object sender, Microsoft.Win32.SessionEndedEventArgs e)
        {
            Stop();

            appIcon.Visible = false;
            Application.Exit();
        }

        private static void StartItem_Click(object sender, EventArgs e)
        {
            appIcon.ContextMenu.MenuItems[0].Enabled = false;
            appIcon.ContextMenu.MenuItems[1].Enabled = true;
            appIcon.Icon = Monitor.Tray.Properties.Resources.ServiceStart;

            Start();
        }

        private static void StopItem_Click(object sender, EventArgs e)
        {
            appIcon.ContextMenu.MenuItems[0].Enabled = true;
            appIcon.ContextMenu.MenuItems[1].Enabled = false;
            appIcon.Icon = Monitor.Tray.Properties.Resources.ServiceStop;

            Stop();
        }

        private static void ExitItem_Click(object sender, EventArgs e)
        {
            Shutdown();

            appIcon.Visible = false;
            Application.Exit();
        }

        private static void Start()
        {
            logger.Info("Monitor started.");

            try
            {
                LoadFile();
            }
            catch (Exception e)
            {
                logger.Error("Load field definition file failed.", e);
                return;
            }

            StartServices();

            LoadJob();
        }

        private static void Stop()
        {
            StopServices();

            logger.Info("Monitor stopped.");
        }

        private static void Shutdown()
        {
            StopServices(true);

            logger.Info("Monitor shutdown.");
        }


        private static void LoadFile()
        {
            switch (SystemInternalSetting.Version)
            {
                case "V47":
                case "V52":
                    new ImportFileProcessor().Parse(@"Fields.csv");
                    break;
                case "GEOpc":
                    new OpcFileProcessor().Parse(@"Fields.csv");
                    break;
                default:    // other should be V47
                    new ImportFileProcessor().Parse(@"Fields.csv");
                    break;
            }
        }

        private static void LoadJob()
        {
            switch (SystemInternalSetting.Version)
            {
                case "V47":
                case "V52":
                    ScheduleEngine.ScheduleScanDirectoryJob();
                    ScheduleEngine.SchedulePurgeDirectoryJob();
                    break;
                case "GEOpc":
                    ScheduleEngine.SchedulePassiveOpcJob();
                    break;
                default:    // other should be V47
                    ScheduleEngine.ScheduleScanDirectoryJob();
                    ScheduleEngine.SchedulePurgeDirectoryJob();
                    break;
            }
        }

        public static bool StartServices()
        {
            //MessageQueueService.StartService();
            //NetworkService.StartService();

            ScheduleEngine.Start();
            logger.Info("Schedule engine started.");

            return true;
        }

        public static bool StopServices(bool isShutdown=false)
        {
            //NetworkService.StopService();
            //MessageQueueService.StopService();

            ScheduleEngine.Stop(isShutdown);
            logger.Info("Schedule engine stopped.");

            return true;
        }
    }
}
