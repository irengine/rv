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
                new ImportFileProcessor().Parse(@"Fields.csv");
            }
            catch (Exception e)
            {
                logger.Error("Load field definition file failed.", e);
                return;
            }

            StartServices();

            ScheduleEngine.ScheduleScanDirectoryJob();
            ScheduleEngine.SchedulePurgeDirectoryJob();

            Console.WriteLine(">>>Enter 'Exit' to exit monitor.");
            Console.Write(">");

            while (!Console.ReadLine().ToUpper().Equals("EXIT"))
            {
                Console.Write(">");
            }

            StopServices();

            logger.Info("Monitor stopped.");
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
