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
*  Create Time : 2023-10-07 15:57:33                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Packets
{
    /// <summary>
    /// 发布释放包
    /// </summary>
    /// <remarks>
    /// PUBREL 封包表示已收到 PUBREC 控制包，它是 QoS 2 协议交换的第 3 个封包。
    /// </remarks>
    public class PubRelPacket : MqttPubAckPacket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public PubRelPacket() : base()
        {
            this.PacketType = PacketType.PUBREL;
        }
        /// <summary>
        /// 设置协议版本
        /// </summary>
        /// <param name="protocolVersion">协议版本</param>
        public PubRelPacket(MqttProtocolVersion protocolVersion) : base(protocolVersion)
        {
            this.PacketType = PacketType.PUBREL;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public PubRelPacket(byte[] buffer) : base(buffer)
        {
            if (buffer == null || buffer.Length == 0) return;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="protocolVersion">协议版本</param>
        public PubRelPacket(byte[] buffer, MqttProtocolVersion protocolVersion) : base(buffer, protocolVersion)
        {
            if (buffer == null || buffer.Length == 0) return;
        }
        #endregion

        #region 属性
        
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override byte GetFlags(int flags = 0)
        {
            return base.GetFlags(0x02);
        }
        #endregion
    }
}