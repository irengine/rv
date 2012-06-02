using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monitor.Schedule.Plugin
{
    public class OPCServerFactory
    {
        public static OPCServer CreateOPCServer()
        {
            return new OPCServer();
        }
    }
}
