using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Mqtt.Internal;
using XiaoFeng.Mqtt.Protocol;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-07 15:57:16                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Packets
{
    /// <summary>
    /// 发布接收包
    /// </summary>
    /// <remarks>
    /// PUBREC 封包表示已收到 PUBLISH 控制包，是对 QoS 为 2 的 PUBLISH 的控制包的阶段 1 应答。它 是 QoS 2 协议交换的第 2 个封包。
    /// </remarks>
    public class PubRecPacket : MqttPubAckPacket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public PubRecPacket() : base()
        {
            this.PacketType = PacketType.PUBREC;
        }
        /// <summary>
        /// 设置协议版本
        /// </summary>
        /// <param name="protocolVersion">协议版本</param>
        public PubRecPacket(MqttProtocolVersion protocolVersion) : base(protocolVersion)
        {
            this.PacketType = PacketType.PUBREC;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public PubRecPacket(byte[] buffer) : base(buffer)
        {
            if (buffer == null || buffer.Length == 0) return;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="protocolVersion">协议版本</param>
        public PubRecPacket(byte[] buffer, MqttProtocolVersion protocolVersion) : base(buffer, protocolVersion)
        {
            if (buffer == null || buffer.Length == 0) return;
        }
        #endregion

        #region 属性

        #endregion

        #region 方法
        
        #endregion
    }
}