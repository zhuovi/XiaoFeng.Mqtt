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
*  Create Time : 2023-10-07 15:56:41                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Packets
{
    /// <summary>
    /// 发布确认包
    /// </summary>
    /// <remarks>
    /// Qos等级为1的 PUBLISH 的应答
    /// </remarks>
    public class PubAckPacket : MqttPubAckPacket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public PubAckPacket() : base()
        {
            this.PacketType = PacketType.PUBACK;
        }
        /// <summary>
        /// 设置协议版本
        /// </summary>
        /// <param name="protocolVersion">协议版本</param>
        public PubAckPacket(MqttProtocolVersion protocolVersion) : base(protocolVersion)
        {
            this.PacketType = PacketType.PUBACK;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public PubAckPacket(byte[] buffer) : base(buffer) { }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="protocolVersion">协议版本</param>
        public PubAckPacket(byte[] buffer, MqttProtocolVersion protocolVersion) : base(buffer, protocolVersion) { }
        #endregion

        #region 属性

        #endregion

        #region 方法
        ///<inheritdoc/>
        public override bool ReadBuffer(MqttBufferReader reader)
        {
            if (this.PacketType != PacketType.PUBACK)
            {
                this.SetError(ReasonCode.MALFORMED_PACKET, $"无效报文.");
                return false;
            }
            return true;
        }
        #endregion
    }
}