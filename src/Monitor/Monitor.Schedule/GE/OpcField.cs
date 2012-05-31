using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monitor.Schedule.GE
{
    class OpcField
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
        string mappingName;

        public string MappingName
        {
            get { return mappingName; }
            set { mappingName = value; }
        }
        string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public OpcField(string[] fs)
        {
            this.index = Int32.Parse(fs[0]);
            this.name = fs[1];
            if (fs.Length > 2)
                this.mappingName = fs[2];
            if (fs.Length > 3)
                this.description = fs[3];
        }
    }
}
