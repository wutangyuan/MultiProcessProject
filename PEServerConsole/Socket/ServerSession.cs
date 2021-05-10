using System.Net.Sockets;
using PENet;


namespace PESocket.Server
{
    /// <summary>
    /// 网络会话连接
    /// </summary>
    public class ServerSession : PESession<PEMessage>
    {
        /// <summary>
        /// 建立会话连接
        /// </summary>
        protected override void OnConnected()
        {
            ServerRoot.Instance.OnConnected(this);
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="msg"></param>
        protected override void OnReciveMsg(PEMessage msg)
        {
            msg.SessionID = (this.skt.RemoteEndPoint as System.Net.IPEndPoint).Address + ":" + (this.skt.RemoteEndPoint as System.Net.IPEndPoint).Port.ToString();
            OnReciveMsgEvent?.Invoke(msg);
        }

        /// <summary>
        /// 断开会话连接
        /// </summary>
        protected override void OnDisConnected()
        {
            string id = (this.skt.RemoteEndPoint as System.Net.IPEndPoint).Address + ":" + (this.skt.RemoteEndPoint as System.Net.IPEndPoint).Port.ToString();
            //  SysLog.Instance.Info(string.Format(@"会话断开-{0}",id));
            ServerSession s = new ServerSession();
            ServerRoot.Instance.ServerSessionList.TryRemove(id, out s);
            System.Collections.Generic.List<string> l = new System.Collections.Generic.List<string>();
            ServerRoot.Instance.ServerSessionDeviceList.TryRemove(id, out l);
        }

        public delegate void OnReciveMsgEventHandler(PEMessage sender);
        public event OnReciveMsgEventHandler OnReciveMsgEvent;
    }
}