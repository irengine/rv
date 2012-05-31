using System.Runtime.InteropServices;
using System;

namespace Monitor.Schedule.V47
{
    public class TagWriter
    {
        private readonly PISDK.PISDK sdk;
        private readonly PISDK.Server server;

        public TagWriter(string serverName)
        {
            this.sdk = new PISDK.PISDK();
            this.server = this.sdk.Servers[serverName];
        }

        public bool AddValue(string tagName, object value)
        {
            PISDK.PIPoint point = this.server.PIPoints[tagName];

            PITimeServer.PITime time = new PITimeServer.PITime();
            time.SetToCurrent();

            try
            {
                point.Data.UpdateValue(value, time);
            }
            catch (COMException)
            {
                return false;
            }

            return true;
        }

        public bool AddValue(string tagName, object value, DateTime timeStamp)
        {
            PISDK.PIPoint point = this.server.PIPoints[tagName];

            try
            {
                // NOTE: Another way to specify the timestamp. 
                point.Data.UpdateValue(value, timeStamp.ToPITime());
            }
            catch (COMException)
            {
                return false;
            }

            return true;
        }
    }

    public static class PIDateTimeExtension
    {
        public static string ToPITime(this DateTime timeStamp)
        {
            // NOTE: Default PI Format
            return timeStamp.ToString("dd-MMM-yy HH:mm:ss");
        }
    }
}
