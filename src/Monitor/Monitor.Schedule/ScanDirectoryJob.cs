using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz;
using Monitor.Common;
using Monitor.Communication;
using System.IO;
using Monitor.Schedule.V47;

namespace Monitor.Schedule
{
    class ScanDirectoryJob : IStatefulJob
    {
        private static ILog logger = LogManager.GetCurrentClassLogger();

        private DateTime lastSearchTime = DateTime.MinValue;
        private string lastFile = "";

        public void Execute(JobExecutionContext context)
        {
            string path = context.JobDetail.JobDataMap["path"].ToString();
            string pattern = context.JobDetail.JobDataMap["pattern"].ToString();
            lastSearchTime = null == context.JobDetail.JobDataMap["lastSearchTime"] ? lastSearchTime : (DateTime)context.JobDetail.JobDataMap["lastSearchTime"];

            logger.Info("Scan directory " + path + "," + pattern);
            ScanDirectory(path, pattern);

            if (string.IsNullOrEmpty(lastFile))
                logger.Info("no new file");
            else
            {
                logger.Info("get new file " + lastFile);
                new DataFileProcessor().Parse(lastFile);
            }

            context.JobDetail.JobDataMap["lastSearchTime"] = lastSearchTime;
        }

        public void ScanDirectory(string path, string pattern)
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
                    string[] files = Directory.GetFiles(path, pattern);
                    foreach (string file in files)
                    {
                        if (File.Exists(file))
                        {
                            //File Info is a Class which extend FileSystemInfo class. 
                            FileInfo fileInfo = new FileInfo(file);
                            if (lastSearchTime < fileInfo.LastWriteTime)
                            {
                                lastSearchTime = fileInfo.LastWriteTime;
                                lastFile = file;
                            }
                        }
                    }
                }
                catch (System.NotSupportedException e)
                {
                    logger.Error("Scan directory failed.", e);
                }
            }
        }
    }
}
