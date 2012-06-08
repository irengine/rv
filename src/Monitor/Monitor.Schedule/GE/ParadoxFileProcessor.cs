using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Monitor.Common;
using Common.Logging;

namespace Monitor.Schedule.GE
{
    public class ParadoxFileProcessor
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
            List<ParadoxField> paradoxFields = new List<ParadoxField>();
            string[] lines = ReadFile(path);

            foreach (string line in lines)
            {
                string[] fs = line.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
                paradoxFields.Add(new ParadoxField(fs));
            }

            foreach (ParadoxField paradoxField in paradoxFields)
            {
                logger.Info(string.Format("{0}\t{1}", paradoxField.Index, paradoxField.Name));
            }

            SystemInternalSetting.Fields = paradoxFields;
        }
    }
}
