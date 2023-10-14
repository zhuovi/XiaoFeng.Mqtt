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
*  Create Time : 2023-10-14 12:36:33                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt
{
    /// <summary>
    /// 消息主题接口
    /// </summary>
    public interface IMqttServerTopicMessage
    {
        /// <summary>
        /// 主题
        /// </summary>
        string Topic { get; set; }
        /// <summary>
        /// 消息包
        /// </summary>
        PublishPacket PublishPacket { get; set; }
        /// <summary>
        /// 分发次数
        /// </summary>
        int DistributeCount { get; }
        /// <summary>
        /// 过期时间
        /// </summary>
        int ExpiredTime { get; set; }
        /// <summary>
        /// 添加分发次数
        /// </summary>
        void AddDistributeCount();
    }
}