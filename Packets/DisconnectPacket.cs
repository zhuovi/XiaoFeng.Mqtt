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
*  Create Time : 2023-10-07 16:00:15                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Packets
{
    /// <summary>
    /// 断开连接包
    /// </summary>
    public class DisconnectPacket : MqttPacket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DisconnectPacket() : base()
        {
            this.PacketType = PacketType.DISCONNECT;
        }
        /// <summary>
        /// 设置协议版本
        /// </summary>
        /// <param name="protocolVersion">协议版本</param>
        public DisconnectPacket(MqttProtocolVersion protocolVersion) : base(protocolVersion)
        {
            this.PacketType = PacketType.DISCONNECT;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public DisconnectPacket(byte[] buffer) : base(buffer)
        {
            if (buffer == null || buffer.Length == 0) return;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="protocolVersion">协议版本</param>
        public DisconnectPacket(byte[] buffer, MqttProtocolVersion protocolVersion) : base(buffer, protocolVersion)
        {
            if (buffer == null || buffer.Length == 0) return;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 原因码
        /// </summary>
        public ReasonCode ReasonCode { get; set; }
        /// <summary>
        /// 原因描述字符串
        /// </summary>
        public string ReasonString { get; set; }
        /// <summary>
        /// 参考服务器
        /// </summary>
        /// <remarks>
        /// <para>Type 为 28（0x1C），Value 为 UTF-8 编码的字符串。用于 Server 向 Client 指示其他可用的 Server。包含多个该字段时，认为是 Protocol Error。</para>
        /// <para>参考 4.13 章节，Server 发送 DISCONECT 控制包时需同时携带原因字 0x9C（Use another Server）或0x9D（Server Moved）和 Server Reference。</para>
        /// </remarks>
        public string ServerReference { get; set; }
        /// <summary>
        /// 会话超时间隔
        /// </summary>
        public uint SessionExpiryInterval { get; set; }
        /// <summary>
        /// 用户属性
        /// </summary>
        /// <remarks>
        /// <para>Type 为 38（0x26），Value 是 UTF-8 编码字符串表示的 Name/Value 对，User Property 允许重复多次呈现多个 Name/Value 对，即使是相同的 Name 也可以出现多次。Server 在添加该字段导致会超过 Client 要求的 Maximum Packet Size 时，不得添加该属性[MQTT-3.14.2-4]</para>
        /// </remarks>
        public List<MqttUserProperty> UserProperties { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override bool WriteBuffer(MqttBufferWriter writer)
        {
            writer.WriteByte((byte)this.ReasonCode);
            if (this.ProtocolVersion == MqttProtocolVersion.V500)
            {
                var writerProperty = new MqttBufferWriter();
                writerProperty.WriteProperty(MqttPropertiesId.ServerReference, this.ServerReference);
                writerProperty.WriteProperty(MqttPropertiesId.ReasonString, this.ReasonString);
                writerProperty.WriteProperty(MqttPropertiesId.SessionExpiryInterval, this.SessionExpiryInterval, 4);
                writerProperty.WriteUserProperties(this.UserProperties);
                writer.Write(writerProperty);
                writerProperty.Dispose();
            }
            return true;
        }
        ///<inheritdoc/>
        public override bool ReadBuffer(MqttBufferReader reader)
        {
            this.ReasonCode = (ReasonCode)reader.ReadByte();
            this.UserProperties = new List<MqttUserProperty>();
            if (this.ProtocolVersion == MqttProtocolVersion.V500)
            {
                var length = reader.ReadVariableByteInteger();
                var endLength = reader.Position + length;
                if (length > 0)
                {
                    while (!reader.EndOfStream && reader.Position < endLength)
                    {
                        var id = (MqttPropertiesId)reader.ReadByte();
                        switch (id)
                        {
                            case MqttPropertiesId.ServerReference:
                                this.ServerReference = reader.ReadString();
                                break;
                            case MqttPropertiesId.ReasonString:
                                this.ReasonString = reader.ReadString();
                                break;
                            case MqttPropertiesId.SessionExpiryInterval:
                                this.SessionExpiryInterval = reader.ReadFourByteInteger();
                                break;
                            case MqttPropertiesId.UserProperty:
                                this.UserProperties.Add(new MqttUserProperty(reader.ReadString(), reader.ReadString()));
                                break;
                            default:
                                throw new MqttProtocolException(string.Format("MQTT Protocol Error: {0}", id));
                        }
                    }
                }
            }
            return true;
        }
        ///<inheritdoc/>
        public override string ToString()
        {
            return $"{this.PacketType}: [ReasonCode={this.ReasonCode}] [ReasonString={this.ReasonString}] [ServerReference={this.ServerReference}] [SessionExpiryInterval={this.SessionExpiryInterval}]";
        }
        #endregion
    }
}