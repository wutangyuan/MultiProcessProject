using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PENet
{
    public enum PECmdCode
    {
        /// <summary>
        /// 数据订阅
        /// </summary>
        Registor = 2001,

        /// <summary>
        /// 客户端心跳
        /// </summary>
        CHeartBeat = 2002,

        /// <summary>
        /// 调用视频
        /// </summary>
        CallCCTV = 2003,

        /// <summary>
        /// 全屏
        /// </summary>
        FullScreen = 2004,

        /// <summary>
        /// 正常
        /// </summary>
        NomalScreen = 2005,

        /// <summary>
        /// 切换组态页面
        /// </summary>
        PageChange = 2006,

        /// <summary>
        /// 打开控制面板
        /// </summary>
        OpenInspectorpanel=2007,

        /// <summary>
        /// 数据发送
        /// </summary>
        DataReply =1001,

        /// <summary>
        /// 服务端心跳
        /// </summary>
        SHeartBeat =1002,

        /// <summary>
        /// 全屏模式切换
        /// </summary>
        ShiftFullMode=1003,

        /// <summary>
        /// 正常模式切换
        /// </summary>
        ShiftNomalMode = 1004,

        /// <summary>
        /// 预案信息
        /// </summary>
        PlanInfo=1005,

        /// <summary>
        /// 巡检停留时间，单位-秒
        /// </summary>
        InspectionInterval= 1006,

        /// <summary>
        /// 切换页面，0-空白页，1-主页，2-巡检，3-预案，4-设备信息，5-热力图
        /// </summary>
        ShiftView= 1007,

        /// <summary>
        /// 巡检模式，1-自动，2-人工
        /// </summary>
        InspectionType=1014,

        /// <summary>
        /// 设备信息
        /// </summary>
        DeviceInfo =1008,

        /// <summary>
        /// 屏幕大小
        /// </summary>
        ScreenRegion=1009,

        /// <summary>
        /// WebSocket地址
        /// </summary>
        WebSocketUrl=1010,

        /// <summary>
        /// 关闭程序
        /// </summary>
        CloseProcess=1011,
        
        /// <summary>
        /// 运营模式切换
        /// </summary>
        ModeChange=1012,

        /// <summary>
        /// 切换全屏
        /// </summary>
        ShiftFullScreen=1013,

        /// <summary>
        /// 设备报警
        /// </summary>
        DeviceAlarm = 1015,

        /// <summary>
        /// 列车到站信号
        /// </summary>
        TrainSignal = 1016,

    }

    public enum PEErrorCode
    {
        None = 0,
    }
}
