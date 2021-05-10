using System;
using PENet;

namespace PENet
{
    [Serializable]
    public class PEMessage 
    {
        public int CmdCode;
        public string Data;
        public int ErrCode;
        public string Message;
        public string SessionID;
    }
}