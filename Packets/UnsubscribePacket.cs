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
*  Create Time : 2023-10-07 15:58:59                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Packets
{
    /// <summary>
    /// 取消订阅请求包
    /// </summary>
    public class UnsubscribePacket : MqttPacketWithIdentifier
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public UnsubscribePacket() : base()
        {
            this.PacketType = PacketType.UNSUBSCRIBE;
        }
        /// <summary>
        /// 设置协议版本
        /// </summary>
        /// <param name="protocolVersion">协议版本</param>
        public UnsubscribePacket(MqttProtocolVersion protocolVersion) : base(protocolVersion)
        {
            this.PacketType = PacketType.UNSUBSCRIBE;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public UnsubscribePacket(byte[] buffer) : base(buffer)
        {
            if (buffer == null || buffer.Length == 0) return;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="protocolVersion">协议版本</param>
        public UnsubscribePacket(byte[] buffer, MqttProtocolVersion protocolVersion) : base(buffer, protocolVersion)
        {
            if (buffer == null || buffer.Length == 0) return;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 自定义属性
        /// </summary>
        public List<MqttUserProperty> UserProperties { get; set; }
        /// <summary>
        /// 取消订阅主题
        /// </summary>
        public List<TopicFilter> TopicFilters { get; set; } = new List<TopicFilter>();
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override bool WriteBuffer(MqttBufferWriter writer)
        {
            writer.WriteTwoByteInteger(this.PacketIdentifier);
            if (this.ProtocolVersion == MqttProtocolVersion.V500)
            {
                if (this.UserProperties != null && this.UserProperties.Count > 0)
                {
                    var writerProperty = new MqttBufferWriter();
                    writerProperty.WriteUserProperties(this.UserProperties);

                    writer.Write(writerProperty);
                    writerProperty.Dispose();
                }
            }
            if (this.TopicFilters != null && this.TopicFilters.Count > 0)
            {
                this.TopicFilters.Each(t =>
                {
                    writer.WriteString(t.Topic);
                    if (this.ProtocolVersion == MqttProtocolVersion.V500)
                    {
                        var options = (byte)t.QualityOfServiceLevel;
                        if (t.NoLocal) options |= 0b0100;
                        if (t.RetainAsPublished) options |= 0b1000;
                        if (t.RetainHandling != RetainHandling.SendAtSubscribe)
                        {
                            options |= (byte)(((byte)t.RetainHandling) << 4);
                        }
                        writer.WriteByte(options);
                    }
                });
            }
            return true;
        }
        ///<inheritdoc/>
        public override bool ReadBuffer(MqttBufferReader reader)
        {
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
                                case MqttPropertiesId.UserProperty:
                                    this.UserProperties.Add(new MqttUserProperty(reader.ReadString(), reader.ReadString()));
                                    break;
                                default:
                                    throw new MqttProtocolException(string.Format("MQTT Protocol Error: {0}", id));
                            }
                        }
                    }
                }
                this.TopicFilters = new List<TopicFilter>();
                while (!reader.EndOfStream)
                {
                    var topic = reader.ReadString();
                    var filter = new TopicFilter(topic);
                    if (this.ProtocolVersion == MqttProtocolVersion.V500)
                    {
                        var options = reader.ReadByte();
                        filter.NoLocal = (options & 0b0100) > 0;
                        filter.RetainAsPublished = (options & 0b1000) > 0;
                        filter.RetainHandling = (RetainHandling)((options >> 4) & 0b0011);
                        filter.QualityOfServiceLevel = (QualityOfServiceLevel)(options & 0b0011);
                    }
                    this.TopicFilters.Add(filter);
                }
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
            return $"{this.PacketType}: [PacketIdentifier={this.PacketIdentifier}] [TopicFilters={this.TopicFilters.Join(",")}]";
        }
        #endregion
    }
}