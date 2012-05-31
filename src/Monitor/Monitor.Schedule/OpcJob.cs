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
                Item[] items = new Item[fields.Count];//定义数据项，即item
                int i;
                log.Debug("register item");
                for (i = 0; i < items.Length; i++)//item初始化赋值
                {
                    //log.Debug(ItemName[i]);
                    items[i] = new Item();//创建一个项Item对象。
                    items[i].ClientHandle = Guid.NewGuid().ToString();//客户端给该数据项分配的句柄。
                    items[i].ItemPath = null; //该数据项在服务器中的路径。
                    items[i].ItemName = ((OpcField)fields[i]).Name; //该数据项在服务器中的名字。
                }

                ItemValueResult[] values = m_server.Read(items);

                log.Debug(m => m("Get result count: {0}", values.Length));

                m_server.Disconnect();
            //}
        }

        /*
        private void ParseResult(ItemValueResult[] values, Hashtable htAllVariables, Hashtable htAllParameters)
        {
            log.Debug("Send data");

            Hashtable htColumns = new Hashtable();
            Hashtable htValues = new Hashtable();

            string time_field_name = htAllParameters["time_col"].ToString();

            foreach (ItemValueResult item in values)
            {
                //log.Debug("Item changed" + item.ItemName.ToString() + ": " + item.Value.ToString());
                string mappingField = htAllVariables[item.ItemName].ToString();
                string[] names = mappingField.Split('*');

                // OPC variable, WindT id, Field Name
                // 07T_GBW_Visu.PV, 743*Data34

                string time_field_value = DateTime.Now.ToString();  //item.Timestamp.ToString()

                log.Debug(m => m("Item name:{0}, value:{1}, timestamp:{2}", item.ItemName, item.Value, item.Timestamp.ToString("o")));

                // if WindT exists in hash table, then append, else add new record into hash table, the value is new time field before.
                if (!htColumns.ContainsKey(names[0]))
                    htColumns[names[0]] = time_field_name + "," + names[1];
                else
                    htColumns[names[0]] = htColumns[names[0]] + "," + names[1];

                if (!htValues.ContainsKey(names[0]))
                    htValues[names[0]] = "'" + time_field_value + "'," + "'" + time_field_value + "','" + item.Value + "'";
                else
                    htValues[names[0]] = htValues[names[0]] + ",'" + item.Value + "'";

            }

            foreach (string machine_id in htColumns.Keys)
            {

                StringBuilder sb = new StringBuilder();

                sb.Append("category_id=").Append(htAllParameters["category_id"]);
                sb.Append("&gathering_category_id=").Append(htAllParameters["gathering_category_id"]);
                sb.Append("&machine_id=").Append(machine_id);
                sb.Append("&action_time=").Append(DateTime.Now.ToString());
                sb.Append("&tbl=").Append(htAllParameters["target_tbl"]);
                sb.Append("&history_tbl=").Append(htAllParameters["target_history_tbl"]);
                sb.Append("&cols=").Append(htColumns[machine_id]);
                sb.Append("&result=").Append(Parse((string)htValues[machine_id], DateTime.Now));

                log.Debug("Report result");
                log.Debug(sb.ToString());
            }

        }

        private static string Parse(string vals, DateTime dt2)
        {
            StringBuilder sb = new StringBuilder();

            String batch = String.Format("{0:yyMMddHHmm}{1}", dt2, 0);
            String group = String.Format("{0:yyMMddHHmm}{1}", dt2, 0);

            sb.Append("'" + batch + "',").Append("'" + group + "',").Append(vals).Append("*");

            return sb.ToString();
        }
        */
    }
}
