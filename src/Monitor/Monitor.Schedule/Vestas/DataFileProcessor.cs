using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Monitor.Common;
using System.Globalization;
using Common.Logging;

namespace Monitor.Schedule.Vestas
{
    public class DataFileProcessor
    {
        private static ILog logger = LogManager.GetCurrentClassLogger();

        protected string _newLine = Environment.NewLine;
        protected string _columnSeparator = "  ";

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
            DateTime retrieveTime = DateTime.ParseExact(Path.GetFileNameWithoutExtension(path), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            logger.Info(retrieveTime.ToLongTimeString());

            string[] lines = ReadFile(path);

            int i = 0;
            foreach (string line in lines)
            {
                try
                {
                    TagWriter writer = null;
                    if (String.IsNullOrEmpty(SystemInternalSetting.SERVER_NAME))
                        logger.Info("PI Server setting is empty.");
                    else
                        writer = new TagWriter(SystemInternalSetting.SERVER_NAME);

                    // skip first line
                    if (i != 0)
                    {
                        logger.Debug(m => m("===No{0}===", i));
                        string[] values = line.Split(new string[] { _columnSeparator }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (ImportField field in SystemInternalSetting.Fields)
                        {
                            string name = "Invalid.Name";
                            if (SystemInternalSetting.Version == "V47")
                                name = "V47." + values[1].Replace("-", ".") + "." + field.Name;
                            else if (SystemInternalSetting.Version == "V52")
                                name = "V52." + values[1].Substring(3).Replace("-", ".") + "." + field.Name;

                            string value = values.Length >= field.Index ? values[field.Index - 1].Trim() : "";
                            if (field.Index != 1 && field.Index != 2)
                            {
                                if (field.Index == 3)
                                    value = value == "RUN" ? "1" : "0";

                                if (field.Index == 30)
                                    value = string.IsNullOrEmpty(value) ? "0" : "1";

                                logger.Debug(m => m("{0}:{1}={2}", field.Index, name, value));
                                if (null != writer)
                                    writer.AddValue(name, value);
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Parse data failed.", e);
                }
                i++;
            }
        }
    }
}
