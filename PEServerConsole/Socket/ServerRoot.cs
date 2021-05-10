//using Hjmos.TestLog;
using PENet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PESocket.Server
{
    public class ServerRoot
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static ServerRoot Instance = new ServerRoot();

        /// <summary>
        /// 会话列表
        /// </summary>
        public ConcurrentDictionary<string, ServerSession> ServerSessionList = new ConcurrentDictionary<string, ServerSession>();

        /// <summary>
        /// 会话订阅数据列表
        /// </summary>
        public ConcurrentDictionary<string, List<string>> ServerSessionDeviceList = new ConcurrentDictionary<string, List<string>>();

        public ServerRoot()
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="serverip"></param>
        /// <param name="serverport"></param>
        public void Init()
        {
            try
            {
                PESocket<ServerSession, PEMessage> server = new PESocket<ServerSession, PEMessage>();
                server.StartAsServer("127.0.0.1", 9527);
               
            }
            catch (Exception ex)
            {
              
            }
        }

        /// <summary>
        /// 新建会话连接
        /// </summary>
        /// <param name="session"></param>
        public void OnConnected(ServerSession session)
        {
            string id = (session.skt.RemoteEndPoint as System.Net.IPEndPoint).Address + ":" + (session.skt.RemoteEndPoint as System.Net.IPEndPoint).Port.ToString();
         
            session.OnReciveMsgEvent += CurrentSession_OnReciveMsgEvent;
            ServerSessionList.TryAdd(id, session);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public void SendMessage(string id, PEMessage message)
        {
            // 广播消息
            if (string.IsNullOrEmpty(id))
            {
                foreach (var item in ServerSessionList)
                {
                    item.Value.SendMsg(message);
                }
            }
            else
            {
                if (ServerSessionList.Keys.Contains(id))
                {
                    ServerSessionList[id].SendMsg(message);
                  
                }
               
            }
        }

        /// <summary>
        /// 接收会话消息
        /// </summary>
        /// <param name="sender"></param>
        private void CurrentSession_OnReciveMsgEvent(PEMessage sender)
        {
         
            switch (sender.CmdCode)
            {
                case (int)PECmdCode.CallCCTV:
                case (int)PECmdCode.NomalScreen:
                case (int)PECmdCode.FullScreen:
                case (int)PECmdCode.ScreenRegion:
                case (int)PECmdCode.WebSocketUrl:
                case (int)PECmdCode.ShiftFullMode:
                case (int)PECmdCode.ShiftNomalMode:
                case (int)PECmdCode.ShiftFullScreen:
                case (int)PECmdCode.ShiftView:
                case (int)PECmdCode.InspectionType:
                case (int)PECmdCode.PlanInfo:
                case (int)PECmdCode.DeviceInfo:
                case (int)PECmdCode.DeviceAlarm:
                case (int)PECmdCode.TrainSignal:
                case (int)PECmdCode.OpenInspectorpanel:
                case (int)PECmdCode.PageChange:
                case (int)PECmdCode.CloseProcess:
                    TransmitCmd(sender.SessionID, sender);
                    break;
                case (int)PECmdCode.CHeartBeat:
                    CHeartBeat(sender.SessionID, sender);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 心跳处理
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sender"></param>
        private void CHeartBeat(string id, PEMessage sender)
        {
            try
            {
             
                if (ServerSessionList.ContainsKey(id))
                {
                    ServerSessionList[id].BridgeID = sender.Data;
                    ServerRoot.Instance.SendMessage(id, new PEMessage { CmdCode = (int)PECmdCode.SHeartBeat, Data = "SHeartBeat" });
                }
            }
            catch (Exception ex)
            {
              
            }
        }

        /// <summary>
        /// 转发命令
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sender"></param>
        private void TransmitCmd(string id, PEMessage sender)
        {
          
            string brigeid = ServerSessionList[id].BridgeID;
            var client = ServerSessionList.Where(p => p.Key != id && p.Value.BridgeID == ServerSessionList[id].BridgeID).FirstOrDefault();
            try
            {
                if (!(client.Value == null))
                    ServerRoot.Instance.SendMessage(client.Key, sender);
            }
            catch (Exception ex)
            {
              
            }
        }
    }
}