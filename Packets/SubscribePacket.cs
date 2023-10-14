using System;
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
*  Create Time : 2023-10-07 15:58:24                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Packets
{
    /// <summary>
    /// 订阅请求包
    /// </summary>
    ///<remarks>
    ///SUBSCRIBE 用于 Client 发往 Server 创建一个或多个订阅（Subscriptions）。每个订阅包含了 Client 一个或多个感兴趣的主题（Topic）。Server 向 Client 发送 PUBLISH 就是在转发与该客户端的订阅中主题过滤器相匹配题的 Application Message。SUBSCRIBE 控制包同样指定了每个订阅的 QoS，指示 Server 向该Client 发布该订阅的 Application Message 时的最大允许的 QoS 等级。
    /// </remarks>
    public class SubscribePacket : MqttPacketWithIdentifier
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SubscribePacket() : base()
        {
            this.PacketType = PacketType.SUBSCRIBE;
        }
        /// <summary>
        /// 设置协议版本
        /// </summary>
        /// <param name="protocolVersion">协议版本</param>
        public SubscribePacket(MqttProtocolVersion protocolVersion) : base(protocolVersion)
        {
            this.PacketType = PacketType.SUBSCRIBE;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public SubscribePacket(byte[] buffer) : base(buffer)
        {
            if (buffer == null || buffer.Length == 0) return;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="protocolVersion">协议版本</param>
        public SubscribePacket(byte[] buffer, MqttProtocolVersion protocolVersion) : base(buffer, protocolVersion)
        {
            if (buffer == null || buffer.Length == 0) return;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 订阅的唯一标识
        /// </summary>
        /// <remarks>
        /// 取值范围[1, 268435455]
        /// </remarks>
        public uint SubscriptionIdentifier { get; set; }
        /// <summary>
        /// 自定义属性
        /// </summary>
        public List<MqttUserProperty> UserProperties { get; set; }
        /// <summary>
        /// 订阅载荷
        /// </summary>
        public List<TopicFilter> TopicFilters { get; set; }= new List<TopicFilter>();
        /// <summary>
        /// DUP标志 设为0。表示该消息第一次被发送
        /// </summary>
        public bool Dup { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override bool WriteBuffer(MqttBufferWriter writer)
        {
            writer.WriteTwoByteInteger(this.PacketIdentifier);
            if (this.ProtocolVersion == MqttProtocolVersion.V500)
            {
                var writerProperty = new MqttBufferWriter();
                if (this.SubscriptionIdentifier > 0)
                    writerProperty.WriteVariableProperty(MqttPropertiesId.SubscriptionIdentifier, this.SubscriptionIdentifier);
                writerProperty.WriteUserProperties(this.UserProperties);
                writer.Write(writerProperty);
                writerProperty.Dispose();
            }

            if (this.TopicFilters != null && this.TopicFilters.Count > 0)
            {
                this.TopicFilters.Each(t =>
                {
                    writer.WriteString(t.Topic);
                    var options = (byte)t.QualityOfServiceLevel;
                    if (this.ProtocolVersion == MqttProtocolVersion.V500)
                    {
                        if (t.NoLocal) options |= 0b0100;
                        if (t.RetainAsPublished) options |= 0b1000;
                        if (t.RetainHandling != RetainHandling.SendAtSubscribe)
                        {
                            options |= (byte)(((byte)t.RetainHandling) << 4);
                        }
                    }
                    writer.WriteByte(options);
                });
            }
            return true;
        }
        ///<inheritdoc/>
        public override bool ReadBuffer(MqttBufferReader reader)
        {
            this.PacketIdentifier = reader.ReadTwoByteInteger();
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
                                case MqttPropertiesId.SubscriptionIdentifier:
                                    this.SubscriptionIdentifier = reader.ReadVariableByteInteger();
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
            }
            this.TopicFilters = new List<TopicFilter>();
            while (!reader.EndOfStream)
            {
                var topic = reader.ReadString();
                var filter = new TopicFilter(topic);
                var options = reader.ReadByte();
                if (this.ProtocolVersion == MqttProtocolVersion.V500)
                { 
                    filter.NoLocal = (options & 0b0100) > 0;
                    filter.RetainAsPublished = (options & 0b1000) > 0;
                    filter.RetainHandling = (RetainHandling)((options >> 4) & 0b0011);
                    options &= 0b0011;
                }
                filter.QualityOfServiceLevel = (QualityOfServiceLevel)options;
                this.TopicFilters.Add(filter);
            }
            return true;
        }
        ///<inheritdoc/>
        public override byte GetFlags(int flags = 0)
        {
            return base.GetFlags(0x02);
        }
        ///<inheritdoc/>
        public override string ToString()
        {
            return $"{this.PacketType}: [PacketIdentifier={this.PacketIdentifier}] [SubscriptionIdentifier={this.SubscriptionIdentifier}] [TopicFilters={TopicFilters.Select(a => a.Topic + "(QoS " + (int)a.QualityOfServiceLevel + ")").Join(",")}]";
        }
        #endregion
    }
}