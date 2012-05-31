using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monitor.Schedule.Vestas
{
    class ImportField
    {
        int index;

        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        int length;

        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        public ImportField(string[] fs)
        {
            this.index = Int32.Parse(fs[0]);
            this.name = fs[1];
            if (fs.Length > 2)
                this.description = fs[2];
            if (fs.Length > 3)
                this.length = Int32.Parse(fs[3]);
        }
    }
}
