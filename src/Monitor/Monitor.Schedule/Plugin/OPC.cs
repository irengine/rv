using System;
using System.Collections.Generic;
using System.Text;
using Opc;
using Opc.Da;
using OpcCom;
using Common.Logging;
using System.Collections;
using Monitor.Schedule.GE;

namespace Monitor.Schedule.Plugin
{
    public class OPC
    {
        private static ILog log = LogManager.GetCurrentClassLogger();

        public OPC()
        {
            isOpen = false;
        }

        private bool isOpen;

        public bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        private IList fields;

        public IList Fields
        {
            get { return fields; }
            set { fields = value; }
        }

        private string serverName;

        public string ServerName
        {
            get { return serverName; }
            set { serverName = value; }
        }

        private string serviceName;

        public string ServiceName
        {
            get { return serviceName; }
            set { serviceName = value; }
        }

        private Hashtable itemsMapping = new Hashtable();

        public Hashtable ItemsMapping
        {
            get { return itemsMapping; }
            set { itemsMapping = value; }
        }

        public void Initialzie(string serverName, string serviceName, IList fields)
        {
            this.ServerName = serverName;
            this.ServiceName = serviceName;
            this.Fields = fields;

            ItemName = new string[this.Fields.Count];
            int i = 0;
            foreach (OpcField field in this.Fields)
            {
                this.ItemsMapping.Add(field.Name, field.MappingName);
                ItemName[i++] = field.Name;
            }
        }

        /// <summary>
        /// 数据刷新时间(毫秒)
        /// </summary>
        public int UpdateRate = 1000;

        /// <summary>
        /// Item列表
        /// </summary>
        public string[] ItemName = null;

        private Opc.Da.Server m_server = null;//定义数据存取服务器
        private Opc.Da.Subscription subscription = null;//定义组对象（订阅者）
        private Opc.Da.SubscriptionState state = null;//定义组（订阅者）状态，相当于OPC规范中组的参数
        private Opc.IDiscovery m_discovery = new OpcCom.ServerEnumerator();//定义枚举基于COM服务器的接口，用来搜索所有的此类服务器。

        public void Start()
        {
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

            //设定组状态
            state = new Opc.Da.SubscriptionState();//组（订阅者）状态，相当于OPC规范中组的参数
            state.Name = "test";//组名
            state.ServerHandle = null;//服务器给该组分配的句柄。
            state.ClientHandle = Guid.NewGuid().ToString();//客户端给该组分配的句柄。
            state.Active = true;//激活该组。
            state.UpdateRate = UpdateRate;//刷新频率为1秒。
            state.Deadband = 0;// 死区值，设为0时，服务器端该组内任何数据变化都通知组。
            state.Locale = null;//不设置地区值。

            //添加组
            subscription = (Opc.Da.Subscription)m_server.CreateSubscription(state);//创建组
            
            //定义item列表
            Item[] items = new Item[ItemName.Length];//定义数据项，即item
            int i;
            log.Debug("register item");
            for (i = 0; i < items.Length; i++)//item初始化赋值
            {
                log.Debug(ItemName[i]);
                items[i] = new Item();//创建一个项Item对象。
                items[i].ClientHandle = Guid.NewGuid().ToString();//客户端给该数据项分配的句柄。
                items[i].ItemPath = null; //该数据项在服务器中的路径。
                items[i].ItemName = ItemName[i]; //该数据项在服务器中的名字。
            }

            //添加Item
            subscription.AddItems(items);

            //注册回调事件
            subscription.DataChanged += new Opc.Da.DataChangedEventHandler(this.OnDataChange);

            try
            {
                //异步读取指定数据
                IRequest quest;
                subscription.Read(subscription.Items, 1, this.OnReadComplete, out quest);
            }
            catch(Exception e)
            {
                log.Debug("ignore it", e);
            }

            this.IsOpen = true;
            log.Debug("OPC Monitor started!");
        }

        public void Stop()
        {
            //取消回调事件
            subscription.DataChanged -= new Opc.Da.DataChangedEventHandler(this.OnDataChange);
            //移除组内item
            subscription.RemoveItems(subscription.Items);
            //结束：释放各资源
            m_server.CancelSubscription(subscription);//m_server前文已说明，通知服务器要求删除组。        
            subscription.Dispose();//强制.NET资源回收站回收该subscription的所有资源。         
            m_server.Disconnect();//断开服务器连接

            this.IsOpen = false;
            log.Debug("OPC Monitor stopped!");
        }

        /// <summary>
        /// 读取数据发生变化时触发该事件
        /// </summary>
        public event DataChangedEventHandler OnDataChanged;

        //DataChange回调
        private void OnDataChange(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            log.Debug("Data event");
            if (OnDataChanged != null)
            {
                log.Debug("Got data");
                OnDataChanged(subscriptionHandle, requestHandle, values);
            }
        }

        //ReadComplete回调
        private void OnReadComplete(object requestHandle, Opc.Da.ItemValueResult[] values)
        {
        }

        //WriteComplete回调
        private void OnWriteComplete(object requestHandle, Opc.IdentifiedResult[] values)
        {
        }
    }
}
