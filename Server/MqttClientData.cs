using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Mqtt.Packets;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-13 00:17:15                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Server
{
    /// <summary>
    /// Mqtt客户端数据
    /// </summary>
    public class MqttClientData
    {
        /// <summary>
        /// 连接包
        /// </summary>
        public ConnectPacket ConnectPacket { get; set; }
        /// <summary>
        /// 订阅包
        /// </summary>
        public List<TopicFilter> TopicFilters { get; set; }
    }
}