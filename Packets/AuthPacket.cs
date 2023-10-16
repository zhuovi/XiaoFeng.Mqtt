using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Mqtt.Internal;
using XiaoFeng.Mqtt.Protocol;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-07 16:00:37                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Packets
{
    /// <summary>
    /// 认证包
    /// </summary>
    public class AuthPacket : MqttPacket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public AuthPacket() : base()
        {
            this.PacketType = PacketType.AUTH;
        }
        /// <summary>
        /// 设置协议版本
        /// </summary>
        /// <param name="protocolVersion">协议版本</param>
        public AuthPacket(MqttProtocolVersion protocolVersion) : base(protocolVersion)
        {
            this.PacketType = PacketType.AUTH;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public AuthPacket(byte[] buffer) : base(buffer)
        {            
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="protocolVersion">协议版本</param>
        public AuthPacket(byte[] buffer, MqttProtocolVersion protocolVersion) : base(buffer, protocolVersion)
        {
        }
        #endregion

        #region 属性
        /// <summary>
        /// 认证原因码
        /// </summary>
        /// <remarks>
        /// <para>当 Reason Code 为 0x00（Success）且没有 Property 时，可以忽略 Reason Code 和 Property Length 字段，这种情况下，AUTH 的 Remaining Length 为 0。</para>
        /// </remarks>
        public ReasonCode ReasonCode { get; set; }
        /// <summary>
        /// 认证方法
        /// </summary>
        /// <remarks>
        /// <para>Type 为 21（0x15），Value 为 UTF-8 编码的字符串，表示认证方法。如果该字段重复出现，认为是Protocol Error。参考 4.12 获取更多扩展认证的信息。</para>
        /// </remarks>
        public string AuthenticationMethod { get; set; }
        /// <summary>
        /// 认证数据
        /// </summary>
        /// <remarks>
        /// <para>Type 为 22（0x16），Value 为包含认证数据的 Binary Data，如果该字段重复出现，认为是 Protocol Error。该字段取值由认证方法定义，参考 4.12 获取更多扩展认证的信息。</para>
        /// </remarks>
        public byte[] AuthenticationData { get; set; }
        /// <summary>
        /// 原因描述字符串
        /// </summary>
        /// <remarks>
        /// <para>Type 为 31（0x1F），Value 是 UTF-8 编码字符串，表示认证失败相关的原因信息（协议说是断开的原因）。其应该是一个出于诊断目的、人类可读的字符串，接收方不必解析其内容。</para>
        /// <para>发送方使用此属性提供更多额外信息给接收方。发送方在添加该字段导致会超过对方要求的 Maximum Packet Size 时，不得添加该属性[MQTT - 3.15.2 - 2]。如果该字段重复出现，认为是 Protocol Error。</para>
        /// </remarks>
        public string ReasonString { get; set; }
        /// <summary>
        /// 用户属性
        /// </summary>
        public List<MqttUserProperty> UserProperties { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override bool ReadBuffer(MqttBufferReader reader)
        {
            if (reader.EndOfStream)
            {
                this.ReasonCode = ReasonCode.SUCCESS;
                return true;
            }
            this.ReasonCode = (ReasonCode)reader.ReadByte();
            if (this.ProtocolVersion == MqttProtocolVersion.V500)
            {
                this.UserProperties = new List<MqttUserProperty>();
                var length = reader.ReadVariableByteInteger();
                var endLength = reader.Position + length;
                if (length > 0)
                {
                    while (!reader.EndOfStream && reader.Position < endLength)
                    {
                        var id = (MqttPropertiesId)reader.ReadByte();
                        switch (id)
                        {
                            case MqttPropertiesId.AuthenticationMethod:
                                this.AuthenticationMethod = reader.ReadString();
                                break;
                            case MqttPropertiesId.AuthenticationData:
                                this.AuthenticationData = reader.ReadBinaryData();
                                break;
                            case MqttPropertiesId.ReasonString:
                                this.ReasonString = reader.ReadString();
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
        public override bool WriteBuffer(MqttBufferWriter writer)
        {
            writer.WriteByte((byte)this.ReasonCode);
            if (this.ProtocolVersion == MqttProtocolVersion.V500)
            {
                var writerProperty = new MqttBufferWriter();
                writerProperty.WriteProperty(MqttPropertiesId.AuthenticationMethod, this.AuthenticationMethod);
                writerProperty.WriteProperty(MqttPropertiesId.AuthenticationData, this.AuthenticationData);
                writerProperty.WriteProperty(MqttPropertiesId.ReasonString, this.ReasonString);
                writerProperty.WriteUserProperties(this.UserProperties);
                writer.Write(writerProperty);
                writerProperty.Dispose();
            }
            return true;
        }
        ///<inheritdoc/>
        public override string ToString()
        {
            return $"{this.PacketType}: [ReasonCode={this.ReasonCode}] [ReasonString={this.ReasonString}] [AuthenticationMethod={this.AuthenticationMethod}]";
        }
        #endregion
    }
}