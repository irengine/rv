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
using Monitor.Schedule.Plugin;
using System.Collections;
using Monitor.Schedule.GE;

namespace Monitor.Schedule
{
    class ParadoxJob : IStatefulJob
    {
        private static ILog logger = LogManager.GetCurrentClassLogger();

        public void Execute(JobExecutionContext context)
        {
            logger.Debug(m => m("Paradox job:{0} has been executed.", context.JobDetail.FullName));

            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string database = dataMap.GetString("database");
            Hashtable fields = (Hashtable)dataMap.Get("fields");

            Console.WriteLine(database);
            QueryParadoxFile(Path.GetDirectoryName(database), Path.GetFileNameWithoutExtension(database), fields);
        }

        private void QueryParadoxFile(string dbPath, string tableName, Hashtable htFields)
        {
            ParadoxTable table = new ParadoxTable(dbPath, tableName);
            ParadoxRecord record = table.Last();

            for (int i = 0; i < table.FieldCount; i++)
            {
                if (htFields.ContainsKey(table.FieldNames[i]))
                {
                    logger.Debug(m=>m("==={0}==={1}==={2}", table.FieldNames[i], htFields[table.FieldNames[i]], record.DataValues[i]));
                    ParseResult(htFields[table.FieldNames[i]].ToString(), record.DataValues[i]);
                }
            }
        }

        private TagWriter writer = null;

        private void ParseResult(string item, object value)
        {
            if (null == writer)
            {
                if (String.IsNullOrEmpty(SystemInternalSetting.SERVER_NAME))
                    logger.Info("PI Server setting is empty.");
                else
                    writer = new TagWriter(SystemInternalSetting.SERVER_NAME);
            }

            if (null != writer)
                writer.AddValue(item, value);
        }
    }
}
