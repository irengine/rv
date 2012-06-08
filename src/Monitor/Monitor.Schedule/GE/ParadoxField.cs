using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monitor.Schedule.GE
{
    class ParadoxField
    {
        int index;

        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        string database;

        public string Database
        {
            get { return database; }
            set { database = value;  }
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

        public ParadoxField(string[] fs)
        {
            this.index = Int32.Parse(fs[0]);
            this.database = fs[1];
            this.name = fs[2];
            if (fs.Length > 3)
                this.mappingName = fs[3];
            if (fs.Length > 4)
                this.description = fs[4];
        }
    }
}
