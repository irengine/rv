using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Common.Logging;
using Opc;
using Opc.Da;
using System.Collections;
using Monitor.Schedule.GE;
using Monitor.Schedule.Vestas;
using Monitor.Common;

namespace Monitor.Schedule
{
    class OpcJob: JobBase
    {
        private static ILog log = LogManager.GetCurrentClassLogger();

        protected override void ExecuteCommand(JobExecutionContext context)
        {
            log.Debug(m => m("OPC job:{0} has been executed.", context.JobDetail.FullName));

            JobDataMap dataMap = context.JobDetail.JobDataMap;

            string servername = dataMap.GetString("server_name");
            string servicename = dataMap.GetString("service_name");
            IList fields = (IList)dataMap.Get("fields");

            Query(servername, servicename, fields);
        }

        private string[] GetItemNames(Hashtable htAllVariables)
        {
            string[] itemNames = new string[htAllVariables.Keys.Count];

            int i = 0;
            foreach (string item in htAllVariables.Keys)
            {
                itemNames[i++] = item;
            }
            htAllVariables.Keys.CopyTo(itemNames, 0);

            return itemNames;
        }

        private void Query(string ServerName, String ServiceName, IList fields)
        {
            Opc.Da.Server m_server = null;

            //using (m_server)
            //{
                Opc.IDiscovery m_discovery = new OpcCom.ServerEnumerator();

                //查询服务器
                Opc.Server[] servers = m_discovery.GetAvailableServers(Specification.COM_DA_20, ServerName, null);
                //daver表示数据存取规范版本，Specification.COMDA_20等于2.0版本。
                //host为计算机名，null表示不需要任何网络安全认证。

                if (servers != null)
                {
                    foreach (Opc.Da.Server server in servers)
                    {
                        log.Info(server.Name);

                        //server即为需要连接的OPC数据存取服务器。
                        if (String.Compare(server.Name, ServerName + "." + ServiceName, true) == 0)//为true忽略大小写
                        {
                            m_server = server;//建立连接。
                            break;
                        }
                    }
                }
                else
                {
                    log.Warn("Can't find server!, " + ServerName + "," + ServiceName);
                    return;
                }

                //连接服务器
                if (m_server != null)//非空连接服务器
                {
                    m_server.Connect();
                }
                else
                {
                    log.Warn("Can't connect OPC server, " + ServerName + "," + ServiceName);
                    return;
                }


                //定义item列表
                Hashtable itemsMapping = new Hashtable();
            
                /*
                // Batch number = 50
                for (int i = 0; i < fields.Count / 50; i++)
                {
                    log.Debug("register item, batch " + i.ToString());
                    Item[] items = new Item[50];
                    for (int j = 0; j < 50; j++)
                    {
                        OpcField field = (OpcField)fields[i * 50 + j];
                        itemsMapping.Add(field.Name, field.MappingName);
                        items[j] = new Item();//创建一个项Item对象。
                        items[j].ClientHandle = Guid.NewGuid().ToString();//客户端给该数据项分配的句柄。
                        items[j].ItemPath = null; //该数据项在服务器中的路径。
                        items[j].ItemName = field.Name; //该数据项在服务器中的名字。
                    }

                    ItemValueResult[] values = m_server.Read(items);

                    log.Debug(m => m("Get result count: {0}", values.Length));

                    ParseResult(values, itemsMapping);

                    System.Threading.Thread.Sleep(100);
                }

                if (fields.Count % 50 != 0)
                {
                    log.Debug("register item, batch " + (fields.Count / 50).ToString());
                    Item[] items = new Item[fields.Count - 50 * (fields.Count / 50)];
                    for (int j = 50 * (fields.Count / 50); j < fields.Count; j++)
                    {
                        OpcField field = (OpcField)fields[j];
                        itemsMapping.Add(field.Name, field.MappingName);
                        items[j - 50 * (fields.Count / 50)] = new Item();//创建一个项Item对象。
                        items[j - 50 * (fields.Count / 50)].ClientHandle = Guid.NewGuid().ToString();//客户端给该数据项分配的句柄。
                        items[j - 50 * (fields.Count / 50)].ItemPath = null; //该数据项在服务器中的路径。
                        items[j - 50 * (fields.Count / 50)].ItemName = field.Name; //该数据项在服务器中的名字。
                    }

                    ItemValueResult[] values = m_server.Read(items);

                    log.Debug(m => m("Get result count: {0}", values.Length));

                    ParseResult(values, itemsMapping);
                }
                */

                Item[] items = new Item[fields.Count];//定义数据项，即item
                int i;
                log.Debug("register item");
                for (i = 0; i < items.Length; i++)//item初始化赋值
                {
                    itemsMapping.Add(((OpcField)fields[i]).Name, ((OpcField)fields[i]).MappingName);
                    items[i] = new Item();//创建一个项Item对象。
                    items[i].ClientHandle = Guid.NewGuid().ToString();//客户端给该数据项分配的句柄。
                    items[i].ItemPath = null; //该数据项在服务器中的路径。
                    items[i].ItemName = ((OpcField)fields[i]).Name; //该数据项在服务器中的名字。
                }

                m_server.Disconnect();
            //}
        }

        private void ParseResult(ItemValueResult[] values, Hashtable itemsMapping)
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
                string name = itemsMapping[item.ItemName].ToString();

                if (null != writer)
                    writer.AddValue(name, item.Value);
 
            }
        }
    }
}
