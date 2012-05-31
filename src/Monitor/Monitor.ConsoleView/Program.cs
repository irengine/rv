using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Monitor.Common;
using Monitor.Communication;
using Monitor.Schedule;
using System.Data;
using Monitor.Schedule.Vestas;
using System.Globalization;
using Monitor.Schedule.GE;

namespace Monitor.ConsoleView
{
    class Program
    {
        private static ILog logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
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

            Console.WriteLine(">>>Enter 'Exit' to exit monitor.");
            Console.Write(">");

            while (!Console.ReadLine().ToUpper().Equals("EXIT"))
            {
                Console.Write(">");
            }

            StopServices();

            logger.Info("Monitor stopped.");
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
                    ScheduleEngine.ScheduleOpcJob();
                    break;
                default:    // other should be V47
                    ScheduleEngine.ScheduleScanDirectoryJob();
                    ScheduleEngine.SchedulePurgeDirectoryJob();
                    break;
            }
        }

        public static bool StartServices()
        {
            // TO BE REMOVED
            //MessageQueueService.StartService();
            //NetworkService.StartService();

            ScheduleEngine.Start();

            return true;
        }

        public static bool StopServices()
        {
            // TO BE REMOVED
            //NetworkService.StopService();
            //MessageQueueService.StopService();

            ScheduleEngine.Stop(true);

            return true;
        }
    }
}
