using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Monitor.Common;
using Common.Logging;

namespace Monitor.Schedule.GE
{
    public class OpcFileProcessor
    {
        private static ILog logger = LogManager.GetCurrentClassLogger();

        protected string _newLine = Environment.NewLine;

        private string[] ReadFile(string path)
        {
            string file;

            if (File.Exists(path) == false)
            {
                throw new Exception("File '" + path + "' does not exist.");
            }

            StreamReader sw = new StreamReader(path);
            file = sw.ReadToEnd();
            sw.Close();

            return file.Split(new string[] { _newLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        public void Parse(string path)
        {
            List<OpcField> opcFields = new List<OpcField>();
            string[] lines = ReadFile(path);

            foreach (string line in lines)
            {
                string[] fs = line.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
                opcFields.Add(new OpcField(fs));
            }

            foreach (OpcField opcField in opcFields)
            {
                logger.Info(string.Format("{0}\t{1}", opcField.Index, opcField.Name));
            }

            SystemInternalSetting.Fields = opcFields;
        }
    }
}
