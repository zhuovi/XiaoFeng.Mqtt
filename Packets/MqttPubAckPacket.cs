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
*  Create Time : 2023-10-13 19:23:04                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Packets
{
    /// <summary>
    /// Mqtt publish 应答包
    /// </summary>
    public class MqttPubAckPacket: MqttPacketWithIdentifier
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MqttPubAckPacket() : base() { }
        /// <summary>
        /// 设置协议版本
        /// </summary>
        /// <param name="protocolVersion">协议版本</param>
        public MqttPubAckPacket(MqttProtocolVersion protocolVersion) : base(protocolVersion) { }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public MqttPubAckPacket(byte[] buffer) : base(buffer) { }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="protocolVersion">协议版本</param>
        public MqttPubAckPacket(byte[] buffer, MqttProtocolVersion protocolVersion) : base(buffer, protocolVersion) { }
        #endregion

        #region 属性
        /// <summary>
        /// 原因码
        /// </summary>
        public ReasonCode ReasonCode { get; set; }
        /// <summary>
        /// 原因信息
        /// </summary>
        public string ReasonString { get; set; }
        /// <summary>
        /// 用户属性
        /// </summary>
        public List<MqttUserProperty> UserProperties { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override bool WriteBuffer(MqttBufferWriter writer)
        {
            writer.WriteTwoByteInteger(this.PacketIdentifier);
            writer.WriteByte((byte)this.ReasonCode);
            if (this.ProtocolVersion == MqttProtocolVersion.V500)
            {
                var writerProperty = new MqttBufferWriter();
                writerProperty.WriteProperty(MqttPropertiesId.ReasonString, this.ReasonString);
                writerProperty.WriteUserProperties(this.UserProperties);

                writer.Write(writerProperty);
                writerProperty.Dispose();
            }
            return true;
        }
        ///<inheritdoc/>
        public override bool ReadBuffer(MqttBufferReader reader)
        {
            if ((int)this.PacketType < 4 || (int)this.PacketType > 7)
            {
                this.SetError(ReasonCode.MALFORMED_PACKET, $"无效报文.");
                return false;
            }
            this.PacketIdentifier = reader.ReadTwoByteInteger();
            if (!reader.EndOfStream)
                this.ReasonCode = (ReasonCode)reader.ReadByte();
            else
            {
                this.ReasonCode = ReasonCode.SUCCESS;
                return true;
            }
            if (this.ProtocolVersion == MqttProtocolVersion.V500)
            {
                if (!reader.EndOfStream)
                {
                    var length = reader.ReadVariableByteInteger();
                    var endLength = reader.Position + length;
                    if (length > 0)
                    {
                        this.UserProperties = new List<MqttUserProperty>();
                        while (!reader.EndOfStream && reader.Position < endLength)
                        {
                            var id = (MqttPropertiesId)reader.ReadByte();
                            switch (id)
                            {
                                case MqttPropertiesId.ReasonString:
                                    this.ReasonString = reader.ReadString();
                                    break;
                                case MqttPropertiesId.UserProperty:
                                    this.UserProperties.Add(new MqttUserProperty(reader.ReadString(), reader.ReadString()));
                                    break;
                                default:
                                    this.SetError(ReasonCode.PROTOCOL_ERROR, $"MQTT Protocol Error: {id}");
                                    return false;
                                    //throw new MqttProtocolException(string.Format("MQTT Protocol Error: {0}", id));
                            }
                        }
                    }
                }
            }
            this.ReasonCode = ReasonCode.SUCCESS;
            return true;
        }
        ///<inheritdoc/>
        public override string ToString()
        {
            return $"{this.PacketType}: [PacketIdentifier={this.PacketIdentifier}] [ReasonCode={this.ReasonCode}] [ReasonString={this.ReasonString}]{(this.PacketStatus == PacketStatus.Error ? $" [ErrorCode={this.ErrorCode}] [ErrorMessage={this.ErrorMessage}]" : "")}";
        }
        #endregion
    }
}