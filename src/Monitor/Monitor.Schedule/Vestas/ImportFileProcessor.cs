using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Monitor.Common;
using Common.Logging;

namespace Monitor.Schedule.V47
{
    public class ImportFileProcessor
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
            List<ImportField> importFields = new List<ImportField>();
            string[] lines = ReadFile(path);

            foreach (string line in lines)
            {
                string[] fs = line.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
                importFields.Add(new ImportField(fs));
            }

            foreach (ImportField importField in importFields)
            {
                logger.Info(string.Format("{0}\t{1}", importField.Index, importField.Name));
            }

            SystemInternalSetting.Fields = importFields;
        }
    }
}
