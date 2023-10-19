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
*  Create Time : 2023-10-07 15:54:35                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Packets
{
    /// <summary>
    /// 发布消息包
    /// </summary>
    public class PublishPacket : MqttPacket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public PublishPacket() : base()
        {
            this.PacketType = PacketType.PUBLISH;
        }
        /// <summary>
        /// 设置协议版本
        /// </summary>
        /// <param name="protocolVersion">协议版本</param>
        public PublishPacket(MqttProtocolVersion protocolVersion) : base(protocolVersion)
        {
            this.PacketType = PacketType.PUBLISH;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public PublishPacket(byte[] buffer) : base(buffer) { }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="protocolVersion">协议版本</param>
        public PublishPacket(byte[] buffer, MqttProtocolVersion protocolVersion) : base(buffer, protocolVersion) { }
        #endregion

        #region 属性
        /// <summary>
        /// DUP标志
        /// </summary>
        /// <remarks>
        /// <para>位置：Byte 1，bit 3。</para>
        /// <para>DUP 标志为 0，表示 Client 或 Server 第一次发送 PUBLISH 控制包；如果为 1，表示本次是早期发送封包的重传封包。</para>
        /// <para>Server 转发收到的 PUBLISH 给其他订阅者时，并不直接使用收到 PUBLISH 中的 DUP 标志。Server外发 PUBLISH 和收到的 PUBLISH 中的 DUP 标志是相互独立的，它的值由 Sender 是否重发 PUBLISH 控制包而定[MQTT-3.3.1-3]。</para>
        /// <para>非规范性描述：</para>
        /// <para>接收者收到包含 DUP 标志为 1 的控制包，并不代表其收到过该封包的早期副本。</para>
        /// <para>DUP 标志与 MQTT 控制包自身相关，而不是控制包中包含的 Application Message。比如当 QoS等级是 1 时，Client 收到一个 DUP 标志为 0，但是却包含了一个重复的 Application Message 但是携带不同 Packet Identifier 的 PUBLISH 控制包。 关于 Packet Identifier 的描述参考 2.2.1。</para>
        /// </remarks>
        public bool Dup { get; set; }
        /// <summary>
        /// QoS等级
        /// </summary>
        /// <remarks>
        /// <para>如果 Server 收到的 PUBLISH 封包中的 QoS 等级大于其在 CONNACK 中返回的 Maximum QoS 的值，Server 给 Client 应答一个携带 Reason Code 为 0x9B（QoS not supported）的 DISCONNECT 控制包。具体参见 4.13 章节错误处理部分。</para>
        /// <para>PUBLISH 控制包的 QoS 值不能设置为 3[MQTT-3.3.1-4]。如果 Client 或 Server 收到了 QoS 为 3 的PUBLISH 控制包，则认为是 Malformed Packet，按照 4.13 章节要求返回一个包含 Reason Code 为 0x81（Malformed Packet）的 DISCONNECT 的控制包作为应答。</para>
        /// </remarks>
        public QualityOfServiceLevel QualityOfServiceLevel { get; set; }
        /// <summary>
        /// Retain标识
        /// </summary>
        /// <remarks>
        /// <para>位置：Byte 1，bit 0。</para>
        /// <para>如果Client发送给Server的PUBLISH封包的RETAIN标志为1，Server必须替换当前已保存的（retained）该主题的 message 并且存储 Application Message[MQTT-3.3.1-5]。因此，Server 可以将保存的 message 传递给（deliver）将来订阅该主题的订阅者。如果 Payload 包含 0 字节的数据，Server 可以正常处理这种情况，它应该删除 该主题 已 经 保 存 的 Message ，因此 将来的 该 主题的订阅者将无法收到这些message[MQTT-3.3.1-6]。Server 不能保存 Payload 为 0 的 message[MQTT-3.3.1-7]。</para>
        /// <para>如果 Client 发送给 Server 的 PUBLISH 控制包的 RETAIN 标志为 0，Server 不能保存该 message，同时不得删除或替换现有的已保存的 message[MQTT-3.3.1-8]。</para>
        /// <para>按照4.13描述，Server给Client返回的CONNACK中的Retain Available为0，但是Server收到了RETAIN标志为 1 的 PUBLISH 封包，Server 将会返回 Reason Code 为 0x9A（Retain not supported）的 DISCONNECT作为给 Client 的应答。</para>
        /// <para>当产生一个新的非共享订阅时，最近保存的并且匹配订阅的主题的 Message，将会按照 Retain Handling Subscription Option 发送给 Client。这些 Messages 发送时 RETAIN 标志被设置为 1。那个已保存的 message 被发送由 Retain Handling Subscription Option 控制。在收到订阅时：</para>
        /// <para>如果 Retain Handling 被设置为 0，Server 必须发送已保存的匹配订阅主题的 messages 给Client[MQTT-3.3.1-9]</para>
        /// <para>如果 Retain Handling 被设置为 1，并且当前订阅并不存在，Server 必须发送匹配订阅主题的已保存的 messages 给 Client,，如果订阅已经存在，Server 不能发送已保存的 messages[MQTT-3.3.1-10]</para>
        /// <para>如果 Retain Handling 被设置为 2，Server 不能发送已保存的 messages</para>
        /// <para>参考 3.8.3.1 章节，获取订阅相关选项的信息。</para>
        /// <para>如果 Server 收到了 RETAIN 标志为 1，QoS 为 0 的 PUBLISH 控制包，它应该为该主题保存该 message，但是也许会在随后的任意时间丢弃保存的 message。如果发生了这样的情况，则该主题将不会存在保存的message。</para>
        /// <para>当某个主题的保存的 message 过期时，这个 message 会被丢弃。</para>
        /// <para>Server转发的Application Message的RETAIN flag标志由订阅选项Retain As Published决定。参考3.8.3.1章节获取更多信息。</para>
        /// <para>如果 Retain As Published 订阅选项为 0，Server 在转发 Application Message 时，RETAIN 标志必须设置为 0，不管其收到的 PUBLISH 控制包中的 RETAIN 标志[MQTT-3.3.1-12]。</para>
        /// <para>如果 Retain As Published 订阅选项为 1，Server 在转发 Application Message 时，RETAIN 标志的取值必须与其收到的 PUBLISH 中的 RETAIN 标志保持一致。</para>
        /// <para>非规范性描述：</para>
        /// <para>当发布者不定期地发送状态消息时，保留消息很有用。 新的非共享订户将收到最新状态。</para>
        /// </remarks>
        public bool Retain { get; set; }
        /// <summary>
        /// 主题名
        /// </summary>
        /// <remarks>
        /// <para>主题名称标识有效载荷数据发布到的信息渠道。</para>
        /// <para>主题名必须是 PUBLISH 控制包可变头的第一个字段（filed），它必须是 1.5.4 定义的 UTF-8 编码字符串[MQTT-3.3.2-1]。</para>
        /// <para>PUBLISH 控制包中的 Topic Name 不能包含通配符[MQTT-3.3.2-2]。</para>
        /// <para>Server 发送的 PUBLISH 控制包中的 Topic Name 必须按照 4.7 章节定义的规则匹配 Client 订阅的 Topic Filter[MQTT-3.3.2-3]。然而，因为允许 Server 映射 Topic Name 外另外一个名称，PUBLISH 中的 Topic Name也许与最初收到 PUBLISH 控制包中的 Topic Name 不一致。</para>
        /// <para>为了降低 PUBLISH 控制包的大小，可以使用主题别名 Topic Alias。主题别名的定义在 3.3.2.3.4 章节。如果 Topic Name 的长度为 0 或者其不是别名，认为是 Protocol Error。</para>
        /// </remarks>
        public string Topic { get; set; }
        /// <summary>
        /// 封包ID
        /// </summary>
        public ushort PacketIdentifier { get; set; }
        /// <summary>
        /// 载荷格式指示符
        /// </summary>
        public MqttPayloadFormatIndicator PayloadFormatIndicator { get; set; }
        /// <summary>
        /// 消息过期时间 单位为秒
        /// </summary>
        /// <remarks>
        /// <para>Type 为 2（0x02），Value 为四个 Byte 的整数，以秒为单位表示 Application Message 的生命周期。</para>
        /// <para>如果 Message Expiry Interval 到期，Server 仍然未向匹配的订阅者发送 Application Message，Server 必须删除该订阅者的 Message 的副本[MQTT-3.3.2-5]。</para>
        /// <para>如果不存在该 Property，则 Application Message 永不过期。</para>
        /// <para>Server 转发给订阅者 Client 时，Message Expiry Interval 必须减去在 Server 停留的时间[MQTT-3.3.2-6]。</para>
        /// <para>参考 4.1 章节，获取更多 stored state 的限制信息。</para>
        /// </remarks>
        public uint MessageExpiryInterval { get; set; }
        /// <summary>
        /// 主题别名
        /// </summary>
        public ushort TopicAlias { get; set; }
        /// <summary>
        /// 响应主题
        /// </summary>
        public string ResponseTopic { get; set; }
        /// <summary>
        /// 相关数据
        /// </summary>
        public byte[] CorrelationData { get; set; }
        /// <summary>
        /// 订阅ID
        /// </summary>
        public List<uint> SubscriptionIdentifiers { get; set; }
        /// <summary>
        /// 用户属性
        /// </summary>
        public List<MqttUserProperty> UserProperties { get; set; }
        /// <summary>
        /// 内容类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 发布数据
        /// </summary>
        public byte[] Payload { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override bool WriteBuffer(MqttBufferWriter writer)
        {
            writer.WriteString(this.Topic);
            if (this.QualityOfServiceLevel == QualityOfServiceLevel.AtLeastOnce || this.QualityOfServiceLevel == QualityOfServiceLevel.ExactlyOnce)
            {
                writer.WriteTwoByteInteger(this.PacketIdentifier);
            }
            if (this.ProtocolVersion == MqttProtocolVersion.V500)
            {
                var writerProperty = new MqttBufferWriter();
                writerProperty.WriteProperty(MqttPropertiesId.ContentType, this.ContentType);
                writerProperty.WriteProperty(MqttPropertiesId.CorrelationData, this.CorrelationData);
                writerProperty.WriteProperty(MqttPropertiesId.MessageExpiryInterval, this.MessageExpiryInterval, 4);
                writerProperty.WriteProperty(MqttPropertiesId.PayloadFormatIndicator, (byte)this.PayloadFormatIndicator);
                writerProperty.WriteProperty(MqttPropertiesId.ResponseTopic, this.ResponseTopic);

                if (this.SubscriptionIdentifiers != null && this.SubscriptionIdentifiers.Count > 0)
                {
                    this.SubscriptionIdentifiers.Each(s =>
                    {
                        writerProperty.WriteVariableProperty(MqttPropertiesId.SubscriptionIdentifier, s);
                    });
                }
                writerProperty.WriteUserProperties(this.UserProperties);
                writerProperty.WriteProperty(MqttPropertiesId.TopicAlias, this.TopicAlias, 2);

                writer.Write(writerProperty);
                writerProperty.Dispose();
            }
            if (this.Payload != null && this.Payload.Length > 0)
                writer.WriteBytes(this.Payload);

            return true;
        }
        ///<inheritdoc/>
        public override bool ReadBuffer(MqttBufferReader reader)
        {
            this.Topic = reader.ReadString();
            if (this.QualityOfServiceLevel > QualityOfServiceLevel.AtMostOnce)
            {
                this.PacketIdentifier = reader.ReadTwoByteInteger();
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
                        this.SubscriptionIdentifiers = new List<uint>();
                        while (!reader.EndOfStream && reader.Position < endLength)
                        {
                            var id = (MqttPropertiesId)reader.ReadByte();
                            switch (id)
                            {
                                case MqttPropertiesId.UserProperty:
                                    this.UserProperties.Add(new MqttUserProperty(reader.ReadString(), reader.ReadString()));
                                    break;
                                case MqttPropertiesId.ContentType:
                                    this.ContentType = reader.ReadString();
                                    break;
                                case MqttPropertiesId.CorrelationData:
                                    this.CorrelationData = reader.ReadBinaryData();
                                    break;
                                case MqttPropertiesId.MessageExpiryInterval:
                                    this.MessageExpiryInterval = reader.ReadFourByteInteger();
                                    break;
                                case MqttPropertiesId.PayloadFormatIndicator:
                                    this.PayloadFormatIndicator = (MqttPayloadFormatIndicator)reader.ReadByte();
                                    break;
                                case MqttPropertiesId.ResponseTopic:
                                    this.ResponseTopic = reader.ReadString();
                                    break;
                                case MqttPropertiesId.TopicAlias:
                                    this.TopicAlias = reader.ReadTwoByteInteger();
                                    break;
                                case MqttPropertiesId.SubscriptionIdentifier:
                                    this.SubscriptionIdentifiers.Add(reader.ReadVariableByteInteger());
                                    break;
                                default:
                                    //reader.Prev();
                                    break;
                            }
                        }
                    }
                }
            }
            if(!reader.EndOfStream)
                this.Payload= reader.ReadBytes();
            return true;
        }
        ///<inheritdoc/>
        public override void SetFlags(int flags)
        {
            this.Dup = ((flags & 0b00001000) >> 3) > 0;
            this.Retain = (flags & 0b00000001) > 0;
            this.QualityOfServiceLevel = (QualityOfServiceLevel)((flags & 0b00000110) >> 1);
        }
        ///<inheritdoc/>
        public override byte GetFlags(int flags = 0)
        {
            if (this.QualityOfServiceLevel == QualityOfServiceLevel.AtMostOnce) this.Dup = false;
            byte fixedHeader = 0x00;
            if (this.Retain) fixedHeader |= 0x01;
            fixedHeader |= (byte)((byte)this.QualityOfServiceLevel << 1);
            if (this.Dup) fixedHeader |= 0x08;
            return base.GetFlags(fixedHeader);
        }
        ///<inheritdoc/>
        public override string ToString()
        {
            return $"{this.PacketType}: [PacketIdentifier={this.PacketIdentifier}] [Topic={this.Topic}] [QoS={(int)this.QualityOfServiceLevel}] [Dup={this.Dup}] [Retain={this.Retain}] [MessageExpiryInterval={this.MessageExpiryInterval}] [TopicAlias={this.TopicAlias}] [ResponseTopic={this.ResponseTopic}] [ContentType={this.ContentType}] [PayloadFormatIndicator={(int)this.PayloadFormatIndicator}] [Payload=...({this.Payload?.Length} bytes)]";
        }
        #endregion
    }
}