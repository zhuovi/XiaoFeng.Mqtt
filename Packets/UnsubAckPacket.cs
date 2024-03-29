﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XiaoFeng.Mqtt.Internal;
using XiaoFeng.Mqtt.Protocol;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-07 15:59:13                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Packets
{
    /// <summary>
    /// 取消订阅确认包
    /// </summary>
    public class UnsubAckPacket : MqttPacketWithIdentifier
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public UnsubAckPacket() : base()
        {
            this.PacketType = PacketType.UNSUBACK;
        }
        /// <summary>
        /// 设置协议版本
        /// </summary>
        /// <param name="protocolVersion">协议版本</param>
        public UnsubAckPacket(MqttProtocolVersion protocolVersion) : base(protocolVersion)
        {
            this.PacketType = PacketType.UNSUBACK;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public UnsubAckPacket(byte[] buffer) : base(buffer) { }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="protocolVersion">协议版本</param>
        public UnsubAckPacket(byte[] buffer, MqttProtocolVersion protocolVersion) : base(buffer, protocolVersion) { }
        #endregion

        #region 属性
        /// <summary>
        /// 自定义属性
        /// </summary>
        public List<MqttUserProperty> UserProperties { get; set; }
        /// <summary>
        /// 原因信息
        /// </summary>
        public string ReasonString { get; set; }
        /// <summary>
        /// 原因码
        /// </summary>
        public List<ReasonCode> ReasonCodes { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override bool WriteBuffer(MqttBufferWriter writer)
        {
            writer.WriteTwoByteInteger(this.PacketIdentifier);
            if (this.ProtocolVersion == MqttProtocolVersion.V500)
            {
                var writerProperty = new MqttBufferWriter();
                writerProperty.WriteProperty(MqttPropertiesId.ReasonString, this.ReasonString);
                writerProperty.WriteUserProperties(this.UserProperties);

                writer.Write(writerProperty);
                writerProperty.Dispose();
            }

            if (this.ReasonCodes != null && this.ReasonCodes.Count > 0)
            {
                this.ReasonCodes.Each(c =>
                {
                    writer.WriteByte((byte)c);
                });
            }
            return true;
        }
        ///<inheritdoc/>
        public override bool ReadBuffer(MqttBufferReader reader)
        {
            if (this.PacketType != PacketType.UNSUBACK)
            {
                this.SetError(ReasonCode.MALFORMED_PACKET, $"无效报文.");
                return false;
            }
            this.PacketIdentifier = reader.ReadTwoByteInteger();

            if (!reader.EndOfStream)
            {
                if (this.ProtocolVersion == MqttProtocolVersion.V500)
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
                this.ReasonCodes = new List<ReasonCode>();
                while (!reader.EndOfStream)
                {
                    this.ReasonCodes.Add((ReasonCode)reader.ReadByte());
                }
            }
            return true;
        }
        ///<inheritdoc/>
        public override string ToString()
        {
            return $"{this.PacketType}: [PacketIdentifier={this.PacketIdentifier}] [ReasonCodes={this.ReasonCodes.Select(a => a.ToString()).Join(",")}] [ReasonString={this.ReasonString}]{(this.PacketStatus == PacketStatus.Error ? $" [ErrorCode={this.ErrorCode}] [ErrorMessage={this.ErrorMessage}]" : "")}";
        }
        #endregion
    }
}