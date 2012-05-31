using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Monitor.Schedule
{
    public class FileProcessor
    {
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

        public void PrintResult(string path)
        {
            string[] lines = ReadFile(path);

            int i = 1;
            foreach (string line in lines)
            {
                //System.Console.WriteLine("L{0}, {1}", i, line);

                if (i == 11)
                    PrintItemName(line);

                i++;
            }

            PrintItemName(lines.Last<string>());
        }

        private void PrintItemName(string line)
        {
            string time = line.Substring(0, 5);
            System.Console.WriteLine("\t Item{0}, {1}", 1, time);

            string[] itemNames = SplitByLength(line.Substring(5), 11);

            int i = 1;
            foreach (string itemName in itemNames)
            {
                System.Console.WriteLine("\t Item{0}, {1}", i, itemName);
                i++;
            }
        }

        string[] SplitByLength(string data, int length)
        {
            string[] ret = new string[(data.Length + length - 1) / length];

            for (int i = 0; i < ret.Length - 1; i++)
            {
                ret[i] = data.Substring(i * length, length);
            }
            ret[ret.Length - 1] = data.Substring((ret.Length - 1) * length);
            return ret;
        }
    }
}
