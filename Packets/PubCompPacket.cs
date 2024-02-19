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
*  Create Time : 2023-10-07 15:57:47                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Packets
{
    /// <summary>
    /// 发布完成包
    /// </summary>
    /// <remarks>
    /// PUBCOMP 封包表示已收到 PUBREL 控制包，它是 QoS 2 协议交换的第 4 个封包。
    /// </remarks>
    public class PubCompPacket : MqttPubAckPacket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public PubCompPacket() : base()
        {
            this.PacketType = PacketType.PUBCOMP;
        }
        /// <summary>
        /// 设置协议版本
        /// </summary>
        /// <param name="protocolVersion">协议版本</param>
        public PubCompPacket(MqttProtocolVersion protocolVersion) : base(protocolVersion)
        {
            this.PacketType = PacketType.PUBCOMP;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public PubCompPacket(byte[] buffer) : base(buffer) { }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="protocolVersion">协议版本</param>
        public PubCompPacket(byte[] buffer, MqttProtocolVersion protocolVersion) : base(buffer, protocolVersion) { }
        #endregion

        #region 属性

        #endregion

        #region 方法
        ///<inheritdoc/>
        public override bool ReadBuffer(MqttBufferReader reader)
        {
            if (this.PacketType != PacketType.PUBCOMP)
            {
                this.SetError(ReasonCode.MALFORMED_PACKET, $"无效报文.");
                return false;
            }
            return true;
        }
        #endregion
    }
}