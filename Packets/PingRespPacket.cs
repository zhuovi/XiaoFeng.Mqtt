using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Mqtt.Internal;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-07 15:59:41                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Packets
{
    /// <summary>
    /// Ping响应包
    /// </summary>
    public class PingRespPacket : MqttPacket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public PingRespPacket() : base()
        {
            this.PacketType = PacketType.PINGRESP;
        }
        /// <summary>
        /// 设置协议版本
        /// </summary>
        /// <param name="protocolVersion">协议版本</param>
        public PingRespPacket(MqttProtocolVersion protocolVersion) : base(protocolVersion)
        {
            this.PacketType = PacketType.PINGRESP;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public PingRespPacket(byte[] buffer) : base(buffer) { }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="protocolVersion">协议版本</param>
        public PingRespPacket(byte[] buffer, MqttProtocolVersion protocolVersion) : base(buffer, protocolVersion) { }
        #endregion

        #region 属性

        #endregion

        #region 方法
        ///<inheritdoc/>
        public override string ToString()
        {
            string version;
            switch (this.ProtocolVersion)
            {
                case MqttProtocolVersion.V310:
                    version = "3.1.0";
                    break;
                case MqttProtocolVersion.V311:
                    version = "3.1.1";
                    break;
                case MqttProtocolVersion.V500:
                    version = "5.0.0";
                    break;
                default:
                    version = "unknow";
                    break;
            }
            return $"{this.PacketType}: [ProtocolVersion={version}]";
        }
        #endregion
    }
}