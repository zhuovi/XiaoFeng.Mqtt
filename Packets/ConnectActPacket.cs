using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Mqtt.Internal;
using XiaoFeng.Mqtt.Protocol;
using XiaoFeng.Net;
using static System.Collections.Specialized.BitVector32;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-28 15:40:50                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Packets
{
    /// <summary>
    /// 响应包
    /// </summary>
    public class ConnectActPacket : MqttPacket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ConnectActPacket() : base()
        {
            this.PacketType = PacketType.CONNACK;
        }
        /// <summary>
        /// 设置协议版本
        /// </summary>
        /// <param name="protocolVersion">协议版本</param>
        public ConnectActPacket(MqttProtocolVersion protocolVersion) : base(protocolVersion)
        {
            this.PacketType = PacketType.CONNACK;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public ConnectActPacket(byte[] buffer) : base(buffer) { }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="protocolVersion">协议版本</param>
        public ConnectActPacket(byte[] buffer, MqttProtocolVersion protocolVersion) : base(buffer, protocolVersion) { }
        #endregion

        #region 属性
        /// <summary>
        /// 原因码 MQTTv5.0
        /// </summary>
        public ReasonCode ReasonCode { get; set; }
        /// <summary>
        /// 额外的信息
        /// </summary>
        /// <remarks>
        /// <para>Type 为 31（0x1F），Value 为 UTF-8 编码字符串。该字符串是人类可读的，以便问题排查。</para>
        /// <para>该字段用于 Server 给 Client 提供额外的信息，当加上该字段后，控制包会超过 Client 指定的 MaximumPacket Size 时，Server 必须放弃添加该字段[MQTT - 3.2.2 - 19]。如果该字段重复出现，认为是 Protocol Error。</para>
        /// </remarks>
        public string ReasonString { get; set; }
        /// <summary>
        /// 用户属性
        /// </summary>
        public List<MqttUserProperty> UserProperties { get; set; }
        /// <summary>
        /// 服务器保持活动 以秒为单位的会话超期间隔
        /// </summary>
        /// <remarks>
        /// <para>Type 为 17（0x11），取值为 4byte 的整数，表示以秒为单位的会话超期间隔。当该字段重复出现时，认为是 Protocol Error。</para>
        ///<para>如果 CONNACK 不存在该字段，Server 使用 CONNECT 中的该字段。如果存在，则 Server 要求 Client使用 CONNACK 中的 Session Expiry Interval 值，而不是 CONNECT 中的值。参考 3.1.2.11.2 章节中，关于如何使用该字段的描述。</para>
        /// </remarks>
        public uint SessionExpiryInterval { get; set; }
        /// <summary>
        /// Server 最大能够并发处理的发布个数
        /// </summary>
        /// <remarks>
        /// <para>Type 为 33（0x21），取值为 4byte 的整数，表示 Server 最大能够并发处理的发布个数。该字段重复出现时，认为是 Protocol Error。</para>
        /// <para>Server 使用该字段限制 Client 的 QoS 为 1 和 QoS 为 2 的并发发布的个数，没有机制限制 QoS 为 0 的发布的个数。</para>
        /// <para>如果没有该字段，默认最大值为 65535。</para>
        /// <para>参考 4.9 章节获取关于流控如何使用该字段的具体细节。</para>
        /// </remarks>
        public ushort ReceiveMaximum { get; set; }
        /// <summary>
        /// Session标识 MQTTv3.1.1
        /// </summary>
        /// <remarks>
        /// <para>位置：Connect Acknowledge Flags 的 bit0。</para>
        /// <para>该标志用于通知 Client，在 Server 侧是否将使用之前由 Client ID 标识的 Session State，这使得 Client 和 Server 对于 Session 保持一致的视图。</para>
        /// <para>如果 Server 接受一个 Clean Start 设置为 1 的 CONNECT，Server 必须设置 CONNACK 中的 Session Flag为 0，除了设置 CONNACK 中的 Reason Code 为 0x00(Success) 之外[MQTT - 3.2.2 - 2]。</para>
        /// <para>如果Server接受一个Clean Start设置为0的CONNECT，并且Server已经存在Client ID为标识的Session State，它必须设置 CONNACK 中的 Session Present 为 1，否则设置 Session Present 为 0。在这两种情况下，Server 必须设置 CONNACK 中的 Reason Code 为 0[MQTT-3.2.2-3]。</para>
        /// <para>如果从 Server 收到的 Session Present 的值与预期不符，Client 按如下处理：</para>
        /// <para>如果 Client 侧未保存 Session State，但是接收到了 Session Present 为 1 的 CONNACK，它必须关闭网络连接[MQTT-3.2.2-4]。如果它希望重启新的 Session，可以设置 Clean Start 为 1。</para>
        /// <para>如果 Client 侧保存了 Session State，但是收到了 Session Present 设置为 0 的 CONNACK，如果它希望继续使用该网络连接，则必须丢弃自身保存的 Session State[MQTT-3.2.2-5]。</para>
        /// <para>如果 Server 发送一个 Reason Code 非 0 的 CONNACK 控制包，则 Session Present 必须设置为0[MQTT-3.2.2-6]。</para>
        /// </remarks>
        public bool IsSessionPresent { get; set; }
        /// <summary>
        /// 服务质量等级
        /// </summary>
        /// <remarks>
        /// <para>Type 为 36（0x24），Value 为 1 个 Byte，取值为 0 或 1。如果该字段重复出现，或者取值不是 0 和 1，则认为是 Protocol Error。如果不存在该字段，则默认值为最大 QoS 值 2。</para>
        /// <para>如果 Server 不支持 QoS 为 1 和 QoS 为 2 的 PUBLISH 控制包，它必须在 CONNACK 中返回其支持的Maximum QoS[MQTT-3.2.2-9]。Server 不支持 QoS 为 1 和 QoS 为 2 的 PUBLISH 控制包时，它仍然必须接受 Requested QoS 为 0、1、2 的 SUBSCRIBE 控制包[MQTT - 3.2.2 - 10]。</para>
        /// <para>如果 Client 收到 Server 的 Maximum QoS，它不能发送 QoS 大于 Server 指定的 Maximum QoS 等级[MQTT-3.2.2-11]。如果 Server 收到了 QoS 等级大于 Maximum QoS 等级的 PUBLISH 控制包，认为是 Protocol Error，此时，返回携带 Reason Code 为 0x9B（QoS not supported）的 DISCONNECT 控制包。</para>
        /// <para>如果 Server 收到的 CONNECT 控制包包含的 Will QoS 等级超过了其能力范围，它必须拒绝此连接。它应该返回携带 0x9B（QoS not supported）的 CONNACK 控制包，并关闭网络连接[MQTT-3.2.2-12]。</para>
        /// <para>非规范性描述：</para>
        /// <para>如果 Client 不需要支持以 QoS 1 和 QoS 2 发送 PUBLISH 控制包，在这种情况下，客户端可以通过在SUBSCRIBE 控制包中，在 Maximum QoS 字段设置其最大支持的 QoS 等级。</para>
        /// </remarks>
        public QualityOfServiceLevel MaximumQoS { get; set; }
        /// <summary>
        /// Server 是否支持保留 messages
        /// </summary>
        /// <remarks>
        /// <para>Type 为 37（0x25），Value 为单个 Byte。表示 Server 是否支持保留 messages。取值为 0 时，表示 Server不支持保留 messages；取值为 1，表示 Server 支持。如果不存在该字段，默认支持保留 messages。如果字段重复出现，或者取值不是 0 和 1，则认为是 Protocol error。</para>
        /// <para>如果 Server 接收到的 CONNECT 控制包包含 Will Message，且 Will Retain 设置为 1，但是 Server 不支持该特性，Server 必须拒绝本次连接请求，并发送 Reason Code 为 0x9A（Retain not supported）的 CONNACK控制包[MQTT-3.2.2-13]。</para>
        /// <para>Client 收到 Server 指示 Retain Available 为 0 时，它不得发送 RETAIN 标志位 1 的 PUBLISH 控制包[MQTT-3.2.2-14]。如果 Server 收到这种封包，认为是 Protocol Error。并发送 Reason Code 为 0x9A（Retainnot supported）的 CONNACK 控制包。</para>
        /// </remarks>
        public bool RetainAvailable { get; set; }
        /// <summary>
        ///  Server 可以接受的最大封包大小
        /// </summary>
        /// <remarks>
        /// <para>Type 为 39（0x27），Value 为四个 Byte 的整数。表示 Server 可以接受的最大封包大小。如果不存在该字段，表示无限制。</para>
        /// <para>如果该字段重复出现或者取值为 0，则认为是 Protocol Error。</para>
        /// <para>封包大小是 2.1.4 章节定义的 MQTT 控制包的所有字节数。Server 使用该字段通知 Client 其能够处理的封包的最大尺寸。</para>
        /// <para>Client 不得发送字节数超过该字段指示的值控制包给 Server[MQTT-3.2.2-15]。如果 Server 收到了这样 的封包，Server 需要返回一个携带 reason code 为 0x95（Packet too large）的 DISCONNECT 控制包。</para>
        /// </remarks>
        public uint MaximumPacketSize { get; set; }
        /// <summary>
        /// 分配的客户端标识符
        /// </summary>
        /// <remarks>
        /// <para>Type 为 18（0x12），Value 为 UTF-8 编码的字符串。如果该字段重复，认为是 Protocol Error。</para>
        /// <para>当 CONNECT 控制包中的 Client Identifier 为 0 长度的字符串时，Server 才会给 Client 分配 Client ID。</para>
        /// <para>如果 Client 使用 0 长度的 Client ID 发起连接，Server 必须以包含 Assigned Client ID 的 CONNACK 应答，Server 保证该 Client ID 未被 Server 侧其他 Session 使用[MQTT-3.2.2-16]。</para>
        /// </remarks>
        public string AssignedClientIdentifier { get; set; }
        /// <summary>
        /// 支持的最大的主题别名取值
        /// </summary>
        /// <remarks>
        /// <para>Type 为 34（0x22），Value 为 2 字节整数。如果该字段重复，认为是 Protocol Error。如果未出现该字段，则默认值为 0。</para>
        /// <para>该字段通知 Client 端，Server 侧可以支持的最大的主题别名取值(主题别名是整数)。Server 使用该字段限制当前连接上可以保持的最大主题别名数。Client 不得在 PUBLISH 控制包中发送超过该字段值的主题别名数[MQTT-3.2.2-17]。取值为 0，表示当前连接上不接受任何主题别名。如果不存在该字段或取值为 0，Client 不得向 Server 发送任何主题别名[MQTT-3.2.2-18]。</para>
        /// </remarks>
        public ushort TopicAliasMaximum { get; set; }
        /// <summary>
        /// 否支持野匹配订阅
        /// </summary>
        /// <remarks>
        /// <para>Type 为 40（0x28），Value 为单个 byte，表示是否支持野匹配订阅：0 表示不支持；1 表示支持。不存在该字段，默认支持。如果该字段重复出现或者取值不是 0 和 1，则认为是 Protocol Error。</para>
        /// <para>如果 Server 不支持野匹配订阅，但是收到了野匹配订阅请求，则按 Protocol Error 处理。Server 应该返回携带 Reason Code 为 0xA2（Wildcard Subscription not supported）的 DISCONNECT 控制包。</para>
        /// <para>即使 Server 支持野匹配订阅，也可以拒绝特定的野匹配订阅请求，这种情况下，Server 可以返回SUBACK，携带 Reason Code 为 0xA2（Wildcard Subscription not supported）。</para>
        /// </remarks>
        public bool WildcardSubscriptionAvailable { get; set; }
        /// <summary>
        /// 是否支持订阅
        /// </summary>
        /// <remarks>
        /// <para>Type 为 41（0x29），Value 为单个 byte，表示是否支持订阅标识 ID：0 表示不支持；1 表示支持。不存在该字段，默认支持。如果该字段重复出现或者取值不是 0 和 1，则认为是 Protocol Error。</para>
        /// <para>如果 Server 不支持 Subscription Identifier，但是收到了包含订阅标识的 SUBSCRIBE，则按 Protocol Error处理。Server 应该返回携带 Reason Code 为 0xA1（Subscription Identifiers not supported）的 DISCONNECT控制包。</para>
        /// </remarks>
        public bool SubscriptionIdentifiersAvailable { get; set; }
        /// <summary>
        /// 是否支持共享订阅
        /// </summary>
        /// <remarks>
        /// <para>Type 为 42（0x2A），Value 为单个 byte，表示是否支持共享订阅：0 表示不支持；1 表示支持。不存 在该字段，默认支持。如果该字段重复出现或者取值不是 0 和 1，则认为是 Protocol Error。</para>
        /// <para>如果 Server 不支持共享订阅，但是收到了包含 Shared Subscription 的 SUBSCRIBE，则按 Protocol Error处理。Server 应该返回携带 Reason Code 为 0x9E（Shared Subscription not supported）的 DISCONNECT 控制包。</para>
        /// </remarks>
        public bool SharedSubscriptionAvailable { get; set; }
        /// <summary>
        /// 服务器保持活动
        /// </summary>
        /// <remarks>
        /// <para>Type 为 19（0x13），Value 为两 byte 的整数，如果 Server 的 CONNACK 包含 Server Keep Alive，则优先使用该值，而不是Client的CONNECT中的Client Keep Alive[MQTT-3.2.2-21]；如果Server未发送Server Keep Alive，Server 必须使用 Client 的 CONNECT 中的 Client Keep Alive 的取值[MQTT-3.2.2-22]。如果重复包含该字段，则认为是 Protocol Error。</para>
        /// <para>非规范性描述：</para>
        /// <para>Server Keep Alive 的主要用途是使服务器通知客户端，它将比客户端指定的 Keep Alive 更早地断开客户端的连接。</para>
        /// </remarks>
        public ushort ServerKeepAlive { get; set; }
        /// <summary>
        /// 创建 Response 主题的基础
        /// </summary>
        /// <remarks>
        /// <para>Type 为 26（0x1A），Value 为 UTF-8 编码的字符串，该字符串作为创建 Response 主题的基础，Client从 Response Informaton 字段创建 Reponse Topic 的方式，不在本规范的范围之内。如果重复包含该字段，则认为是 Protocol Error。</para>
        /// <para>如果 Client 发送 Request Response Information 的值为 1，对于 Server 来说，在 CONNACK 中发送Response Information 可选的。</para>
        /// <para>非规范性描述：</para>
        /// <para>这种方法的常见用法是传递主题树的全局唯一部分，该部分至少在其会话的生存期内为该客户端保留。 这通常不能只是一个随机名称，因为请求客户端和响应客户端都需要获得授权才能使用它。 通常，将其用作特定客户端的主题树的根。 为了使服务器返回此信息，通常需要对其进行正确配置。 使用此机制允许在服务器中而不是在每个客户端中一次完成此配置。</para>
        /// </remarks>
        public string ResponseInformation { get; set; }
        /// <summary>
        /// Client 可以使用的其他 Server
        /// </summary>
        /// <remarks>
        /// <para>Type 为 28（0x1C），Value 为 UTF-8 编码的字符串，表示 Client 可以使用的其他 Server。如果重复包含该字段，则认为是 Protocol Error。</para>
        /// <para>Server 可以在 CONNACK 或 DISCONNECT 控制包中使用该字段，并携带原因码 0x9C(Use another Server)或者是 0x9D（Server moved）。</para>
        /// <para>进一步信息，请参考 4.11 章节中关于 Server 重定向如何使用该字段。</para>
        /// </remarks>
        public string ServerReference { get; set; }
        /// <summary>
        /// 认证方法的名称
        /// </summary>
        /// <remarks>
        /// <para>Type 为 21（0x15），Value 为 UTF-8 编码的字符串，表示认证方法的名称。如果重复包含该字段，则认为是 Protocol Error。进一步参考 4.12 章节关于扩展认证的描述。</para>
        /// </remarks>
        public string AuthenticationMethod { get; set; }
        /// <summary>
        /// 认证方法和认证方法对应的阶段定义
        /// </summary>
        /// <remarks>
        /// <para>Type 为 22（0x16），Value 为 Binary Data，该字段内容由认证方法和认证方法对应的阶段定义。如果重复包含该字段，则认为是 Protocol Error。进一步参考 4.12 章节关于扩展认证的描述。</para>
        /// </remarks>
        public byte[] AuthenticationData { get; set; }
        /// <summary>
        /// 连接响应码  MQTTv5.0
        /// </summary>
        public ConnectReturnCode ReturnCode { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override bool WriteBuffer(MqttBufferWriter writer)
        {
            writer.WriteByte((byte)(IsSessionPresent ? 1 : 0));
            writer.WriteByte((byte)this.ReasonCode);
            if (this.ProtocolVersion == MqttProtocolVersion.V500)
            {
                //自定义属性 v5.0
                var writerProperty = new MqttBufferWriter();
                writerProperty.WriteProperty(MqttPropertiesId.SessionExpiryInterval, this.SessionExpiryInterval, 4);
                writerProperty.WriteProperty(MqttPropertiesId.AuthenticationMethod, this.AuthenticationMethod);
                writerProperty.WriteProperty(MqttPropertiesId.AuthenticationData, this.AuthenticationData);
                writerProperty.WriteProperty(MqttPropertiesId.RetainAvailable, this.RetainAvailable);
                writerProperty.WriteProperty(MqttPropertiesId.ReceiveMaximum, this.ReceiveMaximum, 2);
                if (this.MaximumQoS != QualityOfServiceLevel.ExactlyOnce)
                {
                    writerProperty.WriteProperty(MqttPropertiesId.MaximumQoS, (byte)(this.MaximumQoS == QualityOfServiceLevel.AtLeastOnce ? 1 : 0));
                }
                writerProperty.WriteProperty(MqttPropertiesId.AssignedClientIdentifier, this.AssignedClientIdentifier);
                writerProperty.WriteProperty(MqttPropertiesId.TopicAliasMaximum, this.TopicAliasMaximum, 2);
                writerProperty.WriteProperty(MqttPropertiesId.ReasonString, this.ReasonString);
                writerProperty.WriteProperty
                    (MqttPropertiesId.MaximumPacketSize, this.MaximumPacketSize, 4);
                writerProperty.WriteProperty(MqttPropertiesId.WildcardSubscriptionAvailable, this.WildcardSubscriptionAvailable);
                writerProperty.WriteProperty(MqttPropertiesId.SubscriptionIdentifiersAvailable, this.SubscriptionIdentifiersAvailable);
                writerProperty.WriteProperty(MqttPropertiesId.SharedSubscriptionAvailable, this.SharedSubscriptionAvailable);
                writerProperty.WriteProperty(MqttPropertiesId.ServerKeepAlive, this.ServerKeepAlive, 2);
                writerProperty.WriteProperty(MqttPropertiesId.ResponseInformation, this.ResponseInformation);
                writerProperty.WriteProperty(MqttPropertiesId.ServerReference, this.ServerReference);
                writerProperty.WriteUserProperties(this.UserProperties);

                writer.Write(writerProperty);
                writerProperty.Dispose();
            }
            return true;
        }
        ///<inheritdoc/>
        public override bool ReadBuffer(MqttBufferReader reader)
        {
            if (reader.Length == 0) return false;
            if (this.PacketType != PacketType.CONNACK) return false;
            this.IsSessionPresent = reader.ReadByte() > 0;
            this.ReasonCode = (ReasonCode)reader.ReadByte();
            this.ReturnCode = ToReturnCode(this.ReasonCode);
            if (this.ProtocolVersion == MqttProtocolVersion.V500)
            {
                if (!reader.EndOfStream)
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
                                case MqttPropertiesId.UserProperty:
                                    this.UserProperties.Add(new MqttUserProperty(reader.ReadString(), reader.ReadString()));
                                    break;
                                case MqttPropertiesId.SessionExpiryInterval:
                                    this.SessionExpiryInterval = reader.ReadFourByteInteger();
                                    break;
                                case MqttPropertiesId.AuthenticationMethod:
                                    this.AuthenticationMethod = reader.ReadString();
                                    break;
                                case MqttPropertiesId.AuthenticationData:
                                    this.AuthenticationData = reader.ReadBinaryData();
                                    break;
                                case MqttPropertiesId.ReceiveMaximum:
                                    this.ReceiveMaximum = reader.ReadTwoByteInteger();
                                    break;
                                case MqttPropertiesId.TopicAliasMaximum:
                                    this.TopicAliasMaximum = reader.ReadTwoByteInteger();
                                    break;
                                case MqttPropertiesId.MaximumPacketSize:
                                    this.MaximumPacketSize = reader.ReadFourByteInteger();
                                    break;
                                case MqttPropertiesId.RetainAvailable:
                                    this.RetainAvailable = reader.ReadByte() == 1;
                                    break;
                                case MqttPropertiesId.MaximumQoS:
                                    this.MaximumQoS = (QualityOfServiceLevel)reader.ReadByte();
                                    break;
                                case MqttPropertiesId.AssignedClientIdentifier:
                                    this.AssignedClientIdentifier = reader.ReadString();
                                    break;
                                case MqttPropertiesId.ReasonString:
                                    this.ReasonString = reader.ReadString();
                                    break;
                                case MqttPropertiesId.WildcardSubscriptionAvailable:
                                    this.WildcardSubscriptionAvailable = reader.ReadByte() == 1;
                                    break;
                                case MqttPropertiesId.SubscriptionIdentifiersAvailable:
                                    this.SubscriptionIdentifiersAvailable = reader.ReadByte() == 1;
                                    break;
                                case MqttPropertiesId.SharedSubscriptionAvailable:
                                    this.SharedSubscriptionAvailable = reader.ReadByte() == 1;
                                    break;
                                case MqttPropertiesId.ServerKeepAlive:
                                    this.ServerKeepAlive = reader.ReadTwoByteInteger();
                                    break;
                                case MqttPropertiesId.ResponseInformation:
                                    this.ResponseInformation = reader.ReadString();
                                    break;
                                case MqttPropertiesId.ServerReference:
                                    this.ServerReference = reader.ReadString();
                                    break;
                                default:
                                    return false;
                                    //throw new MqttProtocolException(string.Format("MQTT Protocol Error: {0}", id));
                            }
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 把原因码转换到连接响应码
        /// </summary>
        /// <param name="reasonCode">原因码</param>
        /// <returns>连接响应码</returns>
        private ConnectReturnCode ToReturnCode(ReasonCode reasonCode)
        {
            switch (reasonCode)
            {
                case ReasonCode.SUCCESS:
                    return ConnectReturnCode.ACCEPTED;
                case ReasonCode.BANNED:
                case ReasonCode.NOT_AUTHORIZED:
                    return ConnectReturnCode.REFUSED_NOT_AUTHORIZED;
                case ReasonCode.BAD_AUTHENTICATION_METHOND:
                case ReasonCode.BAD_USERNAME_OR_PASSWORD:
                    return ConnectReturnCode.REFUSED_BAD_USERNAME_OR_PASSWORD;
                case ReasonCode.CLIENT_IDENTIFIER_NOT_VALID:
                    return ConnectReturnCode.REFUSED_IDENTIFIER_REJECTED;
                case ReasonCode.UNSUPPORTED_PROTOCOL_VERSION:
                    return ConnectReturnCode.REFUSED_UNACCEPTABLE_PROTOCOL_VERSION;
                case ReasonCode.USE_ANOTHER_SERVER:
                case ReasonCode.SERVER_UNAVAILABLE:
                case ReasonCode.SERVER_BUSY:
                case ReasonCode.SERVER_MOVED:
                    return ConnectReturnCode.REFUSED_SERVER_UNAVAILABLE;
                default:
                    return ConnectReturnCode.REFUSED_UNACCEPTABLE_PROTOCOL_VERSION;
            }
        }
        /// <summary>
        /// 重写转换字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.PacketType}: [ReturnCode={this.ReturnCode}] [ReasonCode={this.ReasonCode}] [IsSessionPresent={this.IsSessionPresent}] [SessionExpiryInterval={this.SessionExpiryInterval}] [ReceiveMaximum={this.ReceiveMaximum}] [MaximumQoS={(int)this.MaximumQoS}] [RetainAvailable={(this.RetainAvailable ? 1 : 0)}] [MaximumPacketSize={this.MaximumPacketSize}] [AssignedClientIdentifier={this.AssignedClientIdentifier}] [TopicAliasMaximum={this.TopicAliasMaximum}] [WildcardSubscriptionAvailable={(this.WildcardSubscriptionAvailable ? 1 : 0)}] [SubscriptionIdentifiersAvailable={this.SubscriptionIdentifiersAvailable}] [SharedSubscriptionAvailable={this.SharedSubscriptionAvailable}] [ServerKeepAlive={this.ServerKeepAlive}] [ServerReference={this.ServerReference}]";
        }
        #endregion
    }
}