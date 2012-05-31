using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Monitor.Common
{
    public class SystemInternalSetting
    {
        private static ScheduleConfiguration conf = new ScheduleConfiguration();

        public static Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }

        public static string QueryProjectsSQL
        {
            get
            {
                return conf.QUERY_PROJECTS_SQL;
            }
        }

        public static string QueryProjectUpdateSQL
        {
            get
            {
                return conf.QUERY_PROJECT_UPDATE_SQL;
            }
        }

        public static string ConnectionString
        {
            get
            {
                return conf.CONNECTION_STRING;
            }
        }

        public static string Ip
        {
            get { return conf.IP; }
        }

        public static int Port
        {
            get { return conf.PORT; }
        }

        public static string Frequence
        {
            get { return conf.FREQUENCY; }
        }

        private static string aes_iv = "0000000000123456";

        public static string AES_IV
        {
            get { return aes_iv; }
            set { aes_iv = value; }
        }
        private static string aes_key = "0000000000123456";

        public static string AES_KEY
        {
            get { return aes_key; }
            set { aes_key = value; }
        }

        private static Hashtable projects = new Hashtable();

        public static Hashtable Projects
        {
            get { return projects; }
            set { projects = value; }
        }

        private static IList fields;

        public static IList Fields
        {
            get { return SystemInternalSetting.fields; }
            set { SystemInternalSetting.fields = value; }
        }

        public static string ExportPath
        {
            get { return conf.ExportPath; }
        }

        public static string Pattern
        {
            get { return conf.Pattern; }
        }

        public static int PurgeHours
        {
            get { return conf.PurgeHours; }
        }

        public static string Version
        {
            get { return conf.Version; }
        }

        public static string SERVER_NAME
        {
            get { return conf.SERVER_NAME; }
        }

        public static string OpcServerName
        {
            get { return conf.OpcServerName; }
        }

        public static string OpcServiceName
        {
            get { return conf.OpcServiceName; }
        }
    }
}
