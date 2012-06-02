using Common.Logging;
using Opc.Da;
using System.Collections;
using System.Text;
using System;
using Monitor.Schedule.Vestas;
using Monitor.Common;

namespace Monitor.Schedule.Plugin
{
    public class OPCServer
    {
        private static ILog log = LogManager.GetCurrentClassLogger();

        private OPC opc;

        public OPC CurrentOPC
        {
            get
            {
                return opc;
            }
        }

        public OPCServer()
        {
            opc = new OPC();
            opc.OnDataChanged += OnDataChange;
        }

        public void OnDataChange(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            log.Debug("Parse data");

            TagWriter writer = null;
            if (String.IsNullOrEmpty(SystemInternalSetting.SERVER_NAME))
                log.Info("PI Server setting is empty.");
            else
                writer = new TagWriter(SystemInternalSetting.SERVER_NAME);

            foreach (ItemValueResult item in values)
            {
                //log.Debug("Item changed" + item.ItemName.ToString() + ": " + item.Value.ToString());
                string name = this.CurrentOPC.ItemsMapping[item.ItemName].ToString();

                if (null != writer)
                    writer.AddValue(name, item.Value);

            }
        }
    }
}
