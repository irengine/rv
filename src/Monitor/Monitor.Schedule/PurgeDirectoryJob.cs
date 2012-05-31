using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz;
using Monitor.Common;
using Monitor.Communication;
using System.IO;
using Monitor.Schedule.Vestas;

namespace Monitor.Schedule
{
    class PurgeDirectoryJob : IStatefulJob
    {
        private static ILog logger = LogManager.GetCurrentClassLogger();

        public void Execute(JobExecutionContext context)
        {
            string path = context.JobDetail.JobDataMap["path"].ToString();
            string pattern = context.JobDetail.JobDataMap["pattern"].ToString();

            logger.Info("Purge directory " + path + "," + pattern);
            PurgeDirectory(path, pattern);
        }

        public void PurgeDirectory(string path, string pattern)
        {
            //Checks if the path is valid or not
            if (!Directory.Exists(path))
            {
                logger.Error("invalid path");
            }
            else
            {
                try
                {
                    string purgeFileName = DateTime.Now.AddHours(-1 * SystemInternalSetting.PurgeHours).ToString("yyyyMMddHHmmss");

                    logger.Info("Purge file before " + purgeFileName);
                    
                    string[] files = Directory.GetFiles(path, pattern);
                    foreach (string file in files)
                    {
                        if (File.Exists(file) && String.Compare(Path.GetFileNameWithoutExtension(file), purgeFileName) < 0)
                        {
                            File.Delete(file);
                        }
                    }
                }
                catch (System.NotSupportedException e)
                {
                    logger.Error("Purge directory failed.", e);
                }
            }
        }
    }
}
