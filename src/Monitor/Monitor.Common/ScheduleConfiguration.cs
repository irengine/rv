﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Monitor.Common
{
    public class ScheduleConfiguration : wwAppConfiguration
    {
        public string Code;
        public DateTime LastTime;
        public string FREQUENCY = "0 0/5 * * * ?";
        public string IP = "127.0.0.1";
        public int PORT = 4400;
        public string QUERY_PROJECTS_SQL = "QUERY_PROJECTS_SQL";
        public string QUERY_PROJECT_UPDATE_SQL = "QUERY_PROJECT_UPDATE_SQL";
        public string CONNECTION_STRING = "localhost";
        public string ExportPath;
        public string Pattern;
        public int PurgeHours;
        public string Version;
        public string SERVER_NAME;

        public string OpcServerName;
        public string OpcServiceName;

        public ScheduleConfiguration()
            : base(false)
        {
            this.ReadKeysFromConfig();
        }

        public ScheduleConfiguration(string code)
            : base(false)
        {
            this.Code = code;
            this.SetConfigurationSection(this.Code);
            this.ReadKeysFromConfig();
        }

        public override void ReadKeysFromConfig()
        {
            ReadKeysFromConfig(GetFileName());
        }

        public override bool WriteKeysToConfig()
        {
            return WriteKeysToConfig(GetFileName());
        }

        private string GetFileName()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Schedule.conf");
        }
    }
}
