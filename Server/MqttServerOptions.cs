using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-12 11:23:21                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Server
{
    /// <summary>
    /// Mqtt服务端配置
    /// </summary>
    public class MqttServerOptions
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MqttServerOptions()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否启用Tls
        /// </summary>
        public bool UseTls { get; set; }
        /// <summary>
        /// 证书
        /// </summary>
        public X509Certificate Certificate { get; set; }
        /// <summary>
        /// 协议版本
        /// </summary>
        public SslProtocols SslProtocols { get; set; }
        /// <summary>
        /// 连接超时时间 单位秒
        /// </summary>
        public int ConnectTimeout { get; set; }
        /// <summary>
        /// 读超时时间 单位秒
        /// </summary>
        public int ReadTimout { get; set; }
        /// <summary>
        /// 写超时时间 单位秒
        /// </summary>
        public int WriteTimeout { get; set; }
        /// <summary>
        /// 读缓冲区大小
        /// </summary>
        public int ReadBufferSize { get; set; } = 8192;
        /// <summary>
        /// 写缓冲区大小
        /// </summary>
        public int WriteBufferSize { get; set; } = 8192;
        /// <summary>
        /// 终结点
        /// </summary>
        private IPEndPoint _EndPoint;
        /// <summary>
        /// 终结点
        /// </summary>
        public IPEndPoint EndPoint
        {
            get
            {
                if (this._EndPoint == null)
                    this._EndPoint = new IPEndPoint(IPAddress.Any, 1883);
                return this._EndPoint;
            }
            set => this._EndPoint = value;
        }
        /// <summary>
        /// 服务器保持活动 以秒为单位的会话超期间隔
        /// </summary>
        public uint SessionExpiryInterval { get; set; }
        /// <summary>
        /// Server 最大能够并发处理的发布个数
        /// </summary>
        /// <remarks>
        /// <para>Server 使用该字段限制 Client 的 QoS 为 1 和 QoS 为 2 的并发发布的个数，没有机制限制 QoS 为 0 的发布的个数。</para>
        /// <para>如果没有该字段，默认最大值为 65535。</para>
        /// </remarks>
        public ushort ReceiveMaximum { get; set; }
        /// <summary>
        /// 服务质量等级
        /// </summary>
        /// <remarks>
        /// <para>如果 Client 收到 Server 的 Maximum QoS，它不能发送 QoS 大于 Server 指定的 Maximum QoS 等级[MQTT-3.2.2-11]。如果 Server 收到了 QoS 等级大于 Maximum QoS 等级的 PUBLISH 控制包，认为是 Protocol Error，此时，返回携带 Reason Code 为 0x9B（QoS not supported）的 DISCONNECT 控制包。</para>
        /// </remarks>
        public QualityOfServiceLevel MaximumQoS { get; set; } = QualityOfServiceLevel.ExactlyOnce;
        /// <summary>
        /// Server 是否支持保留 messages
        /// </summary>
        /// <remarks>
        /// <para>如果 Server 接收到的 CONNECT 控制包包含 Will Message，且 Will Retain 设置为 1，但是 Server 不支持该特性，Server 必须拒绝本次连接请求，并发送 Reason Code 为 0x9A（Retain not supported）的 CONNACK控制包[MQTT-3.2.2-13]。</para>
        /// <para>Client 收到 Server 指示 Retain Available 为 0 时，它不得发送 RETAIN 标志位 1 的 PUBLISH 控制包[MQTT-3.2.2-14]。如果 Server 收到这种封包，认为是 Protocol Error。并发送 Reason Code 为 0x9A（Retain not supported）的 CONNACK 控制包。</para>
        /// </remarks>
        public bool RetainAvailable { get; set; } = true;
        /// <summary>
        ///  Server 可以接受的最大封包大小
        /// </summary>
        /// <remarks>
        /// <para>封包大小是 2.1.4 章节定义的 MQTT 控制包的所有字节数。Server 使用该字段通知 Client 其能够处理的封包的最大尺寸。</para>
        /// <para>Client 不得发送字节数超过该字段指示的值控制包给 Server[MQTT-3.2.2-15]。如果 Server 收到了这样 的封包，Server 需要返回一个携带 reason code 为 0x95（Packet too large）的 DISCONNECT 控制包。</para>
        /// </remarks>
        public uint MaximumPacketSize { get; set; }
        /// <summary>
        /// 支持的最大的主题别名取值
        /// </summary>
        /// <remarks>
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
        public bool WildcardSubscriptionAvailable { get; set; } = true;
        /// <summary>
        /// 是否支持订阅
        /// </summary>
        /// <remarks>
        /// <para>Type 为 41（0x29），Value 为单个 byte，表示是否支持订阅标识 ID：0 表示不支持；1 表示支持。不存在该字段，默认支持。如果该字段重复出现或者取值不是 0 和 1，则认为是 Protocol Error。</para>
        /// <para>如果 Server 不支持 Subscription Identifier，但是收到了包含订阅标识的 SUBSCRIBE，则按 Protocol Error处理。Server 应该返回携带 Reason Code 为 0xA1（Subscription Identifiers not supported）的 DISCONNECT控制包。</para>
        /// </remarks>
        public bool SubscriptionIdentifiersAvailable { get; set; } = true;
        /// <summary>
        /// 是否支持共享订阅
        /// </summary>
        /// <remarks>
        /// <para>Type 为 42（0x2A），Value 为单个 byte，表示是否支持共享订阅：0 表示不支持；1 表示支持。不存 在该字段，默认支持。如果该字段重复出现或者取值不是 0 和 1，则认为是 Protocol Error。</para>
        /// <para>如果 Server 不支持共享订阅，但是收到了包含 Shared Subscription 的 SUBSCRIBE，则按 Protocol Error处理。Server 应该返回携带 Reason Code 为 0x9E（Shared Subscription not supported）的 DISCONNECT 控制包。</para>
        /// </remarks>
        public bool SharedSubscriptionAvailable { get; set; } = true;
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
        /// 是否允许匿名访问
        /// </summary>
        public bool AllowAnonymousAccess { get; set; } = true;
        /// <summary>
        /// 最大分发次数
        /// </summary>
        public int MaximumDistribute { get; set; } = 10;
        /// <summary>
        /// 保存主题消息过期时间 单位秒
        /// </summary>
        public int TopicFilterExpireInterval { get; set; } = 600;
        #endregion

        #region 方法

        #endregion
    }
}