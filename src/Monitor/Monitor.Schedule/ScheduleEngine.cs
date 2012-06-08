using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Quartz.Impl;
using Common.Logging;
using Monitor.Common;
using Monitor.Schedule.Vestas;
using Monitor.Schedule.Plugin;
using System.Collections;
using Monitor.Schedule.GE;

namespace Monitor.Schedule
{
    public class ScheduleEngine
    {
        private static ILog logger = LogManager.GetCurrentClassLogger();

        private static IScheduler engine = null;

        private static void Initialize()
        {
            if (engine == null)
            {
                ISchedulerFactory sf = new StdSchedulerFactory();
                engine = sf.GetScheduler();
            }
        }

        public static void Start()
        {
            Initialize();
            engine.Start();
            logger.Info("Schedule engine started.");
        }

        public static void Stop(bool isShutdown)
        {
            if (isShutdown)
            {
                engine.Shutdown(true);
                logger.Info("Schedule engine shutdown.");
            }
            else
            {
                engine.Standby();
                logger.Info("Schedule engine stopped.");
            }
        }

        public static void Schedule()
        {

        }

        public static string ONE_MINUTE_PATTERN = "0 0/1 * * * ?";
        public static string FIVE_MINUTE_PATTERN = "0 0/5 * * * ?";
        public static string ONE_HOUR_PATTERN = "30 0/60 * * * ?";

        public static void SchedulePurgeDirectoryJob()
        {
            string jobName = "PurgeDirectory";
            JobDetail job = new JobDetail(jobName, jobName, typeof(PurgeDirectoryJob));
            job.JobDataMap["path"] = SystemInternalSetting.ExportPath;
            job.JobDataMap["pattern"] = SystemInternalSetting.Pattern;

            CronTrigger trigger = new CronTrigger(jobName, jobName, jobName, jobName, ONE_HOUR_PATTERN);
            engine.AddJob(job, true);
            DateTime ft = engine.ScheduleJob(trigger);
        }

        public static void ScheduleScanDirectoryJob()
        {
            string jobName = "ScanDirectory";
            JobDetail job = new JobDetail(jobName, jobName, typeof(ScanDirectoryJob));
            job.JobDataMap["path"] = SystemInternalSetting.ExportPath;
            job.JobDataMap["pattern"] = SystemInternalSetting.Pattern;

            CronTrigger trigger = new CronTrigger(jobName, jobName, jobName, jobName, SystemInternalSetting.Frequence);
            engine.AddJob(job, true);
            DateTime ft = engine.ScheduleJob(trigger);
        }

        public static void ScheduleActiveOpcJob()
        {
            string jobName = "ScanOpc";
            JobDetail job = new JobDetail(jobName, jobName, typeof(OpcJob));
            job.JobDataMap["server_name"] = SystemInternalSetting.OpcServerName;
            job.JobDataMap["service_name"] = SystemInternalSetting.OpcServiceName;
            job.JobDataMap["fields"] = SystemInternalSetting.Fields;

            CronTrigger trigger = new CronTrigger(jobName, jobName, jobName, jobName, SystemInternalSetting.Frequence);
            engine.AddJob(job, true);
            DateTime ft = engine.ScheduleJob(trigger);
        }

        public static void SchedulePassiveOpcJob()
        {
            OPCServer opcSrv = OPCServerFactory.CreateOPCServer();
            if (opcSrv.CurrentOPC.IsOpen)
            {
                opcSrv.CurrentOPC.Stop();
            }

            // Initialize...
            opcSrv.CurrentOPC.Initialzie(SystemInternalSetting.OpcServerName, SystemInternalSetting.OpcServiceName, SystemInternalSetting.Fields);

            opcSrv.CurrentOPC.Start();
        }

        public static void BatchScheduleParadoxJob()
        {
            // database = path + "," + table name(no extension)
            Hashtable htParadox = new Hashtable();

            IList fields = SystemInternalSetting.Fields;

            // reload data
            foreach (ParadoxField field in fields)
            {
                if (!htParadox.ContainsKey(field.Database))
                {
                    htParadox[field.Database] = new Hashtable();
                }

                ((Hashtable)htParadox[field.Database])[field.Name] = field.MappingName;
            }

            foreach (string database in htParadox.Keys)
            {
                ScheduleParadoxJob(database, (Hashtable)htParadox[database]);
                //QueryParadoxFile(Path.GetDirectoryName(database), Path.GetFileNameWithoutExtension(database), (Hashtable)htParadox[database]);
            }
        }

        private static void ScheduleParadoxJob(string database, Hashtable fields)
        {
            string jobName = "Paradox" + "," + database;
            JobDetail job = new JobDetail(jobName, jobName, typeof(ParadoxJob));
            job.JobDataMap["database"] = database;
            job.JobDataMap["fields"] = fields;

            CronTrigger trigger = new CronTrigger(jobName, jobName, jobName, jobName, SystemInternalSetting.Frequence);
            engine.AddJob(job, true);
            DateTime ft = engine.ScheduleJob(trigger);
        }

        public static void ScheduleHeartbeatJob(string projectId)
        {
            string jobName = "Heartbeat_" + projectId;
            JobDetail job = new JobDetail(jobName, jobName, typeof(HeartbeatJob));
            job.JobDataMap["projectId"] = projectId;

            CronTrigger trigger = new CronTrigger(jobName, jobName, jobName, jobName, FIVE_MINUTE_PATTERN);
            engine.AddJob(job, true);
            DateTime ft = engine.ScheduleJob(trigger);
        }

        public static void ScheduleSqlJob(string projectId)
        {
            string jobName = "Report_" + projectId;
            JobDetail job = new JobDetail(jobName, jobName, typeof(SqlJob));
            job.JobDataMap["projectId"] = projectId;
            job.JobDataMap["lastTime"] = Monitor.Common.ScheduleSetting.GetLastTime(projectId);

            CronTrigger trigger = new CronTrigger(jobName, jobName, jobName, jobName, Monitor.Common.SystemInternalSetting.Frequence);
            engine.AddJob(job, true);
            DateTime ft = engine.ScheduleJob(trigger);
        }
    }
}
