using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using XiaoFeng.Mqtt.Internal;
using XiaoFeng.Mqtt.Protocol;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-06-16 16:43:16                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Packets
{
    /// <summary>
    /// CONNECT – 连接服务端  报文
    /// </summary>
    /// <remarks>
    /// 客户端到服务端的网络连接建立后，客户端发送给服务端的第一个报文必须是CONNECT报文
    /// 在一个网络连接上，客户端只能发送一次CONNECT报文。
    /// 服务端必须将客户端发送的第二个CONNECT报文当作协议违规处理并断开客户端的连接。
    /// 有效载荷包含一个或多个编码的字段。包括客户端的唯一标识符，Will主题，Will消息，用户名和密码。
    /// 除了客户端标识之外，其它的字段都是可选的，基于标志位来决定可变报头中是否需要包含这些字段。
    /// </remarks>
    public class ConnectPacket : MqttPacket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ConnectPacket() : base()
        {
            this.PacketType = PacketType.CONNECT;
        }
        /// <summary>
        /// 设置协议版本
        /// </summary>
        /// <param name="protocolVersion">协议版本</param>
        public ConnectPacket(MqttProtocolVersion protocolVersion) : base(protocolVersion)
        {
            this.PacketType = PacketType.CONNECT;
        }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public ConnectPacket(byte[] buffer) : base(buffer) { }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="protocolVersion">协议版本</param>
        public ConnectPacket(byte[] buffer, MqttProtocolVersion protocolVersion) : base(buffer, protocolVersion) { }
        #endregion

        #region 属性
        /// <summary>
        /// 协议名称
        /// </summary>
        /// <remarks>
        /// 协议名是表示协议名 MQTT 的UTF-8编码的字符串。MQTT规范的后续版本不会改变这个字符串的偏移和长度。
        /// 如果协议名不正确服务端可以断开客户端的连接，也可以按照某些其它规范继续处理CONNECT报文。对于后一种情况，按照本规范，服务端不能继续处理CONNECT报文 
        /// </remarks>
        public string ProtocolName { get; set; } = "MQTT";
        /// <summary>
        /// 清理会话
        /// </summary>
        /// <remarks>
        /// 位置：连接标志字节的第1位
        /// 这个二进制位指定了会话状态的处理方式。
        /// 客户端和服务端可以保存会话状态，以支持跨网络连接的可靠消息传输。这个标志位用于控制会话状态的生存时间。
        /// 如果清理会话（CleanSession）标志被设置为0，服务端必须基于当前会话（使用客户端标识符识别）的状态恢复与客户端的通信。
        /// 如果没有与这个客户端标识符关联的会话，服务端必须创建一个新的会话。
        /// 在连接断开之后，当连接断开后，客户端和服务端必须保存会话信息。
        /// 当清理会话标志为0的会话连接断开之后，服务端必须将之后的QoS 1和QoS 2级别的消息保存为会话状态的一部分，如果这些消息匹配断开连接时客户端的任何订阅。
        /// 服务端也可以保存满足相同条件的QoS 0级别的消息。
        /// 如果清理会话（CleanSession）标志被设置为1，客户端和服务端必须丢弃之前的任何会话并开始一个新的会话。
        /// 会话仅持续和网络连接同样长的时间。
        /// 与这个会话关联的状态数据不能被任何之后的会话重用。
        /// 在V5.0中被叫作Clean Start
        /// </remarks>
        public Boolean CleanSession { get; set; }
        /// <summary>
        /// 遗嘱标志
        /// </summary>
        /// <remarks>
        /// 位置：连接标志的第2位。
        /// 遗嘱标志（Will Flag）被设置为1，表示如果连接请求被接受了，遗嘱（Will Message）消息必须被存储在服务端并且与这个网络连接关联。之后网络连接关闭时，服务端必须发布这个遗嘱消息，除非服务端收到DISCONNECT报文时删除了这个遗嘱消息。
        /// </remarks>
        public Boolean WillFlag { get; set; }
        /// <summary>
        /// 遗嘱QoS
        /// </summary>
        /// <remarks>
        /// 位置：连接标志的第4和第3位。
        /// 这两位用于指定发布遗嘱消息时使用的服务质量等级。
        /// 如果遗嘱标志被设置为0，遗嘱QoS也必须设置为0(0x00)。
        /// 如果遗嘱标志被设置为1，遗嘱QoS的值可以等于0(0x00)，1(0x01)，2(0x02)。
        /// 它的值不能等于3。
        /// </remarks>
        public QualityOfServiceLevel WillQoS { get; set; } = QualityOfServiceLevel.AtLeastOnce;
        /// <summary>
        /// 遗嘱保留
        /// </summary>
        /// <remarks>
        /// 位置：连接标志的第5位。
        /// 如果遗嘱消息被发布时需要保留，需要指定这一位的值。
        /// 如果遗嘱标志被设置为0，遗嘱保留（Will Retain）标志也必须设置为0。
        /// 如果遗嘱标志被设置为1：
        /// 如果遗嘱保留被设置为0，服务端必须将遗嘱消息当作非保留消息发布。
        /// 如果遗嘱保留被设置为1，服务端必须将遗嘱消息当作保留消息发布。
        /// </remarks>
        public Boolean WillRetain { get; set; }
        /// <summary>
        /// 用户名标志
        /// </summary>
        /// <remarks>
        /// 位置：连接标志的第7位。
        /// 如果用户名（User Name）标志被设置为0，有效载荷中不能包含用户名字段。
        /// 如果用户名（User Name）标志被设置为1，有效载荷中必须包含用户名字段。
        /// </remarks>
        public Boolean UserNameFlag { get; set; }
        /// <summary>
        /// 密码标志
        /// </summary>
        /// <remarks>
        /// 位置：连接标志的第6位。
        /// 如果密码（Password）标志被设置为0，有效载荷中不能包含密码字段。
        /// 如果密码（Password）标志被设置为1，有效载荷中必须包含密码字段。
        /// 如果用户名标志被设置为0，密码标志也必须设置为0。
        /// </remarks>
        public Boolean PasswordFlag { get; set; }
        /// <summary>
        /// 保持连接
        /// </summary>
        /// <remarks>
        /// 保持连接（Keep Alive）是一个以秒为单位的时间间隔，表示为一个16位的字，它是指在客户端传输完成一个控制报文的时刻到发送下一个报文的时刻，两者之间允许空闲的最大时间间隔。客户端负责保证控制报文发送的时间间隔不超过保持连接的值。如果没有任何其它的控制报文可以发送，客户端必须发送一个PINGREQ报文。
        /// 不管保持连接的值是多少，客户端任何时候都可以发送PINGREQ报文，并且使用PINGRESP报文判断网络和服务端的活动状态。
        /// 如果保持连接的值非零，并且服务端在一点五倍的保持连接时间内没有收到客户端的控制报文，它必须断开客户端的网络连接，认为网络连接已断开。
        /// 客户端发送了PINGREQ报文之后，如果在合理的时间内仍没有收到PINGRESP报文，它应该关闭到服务端的网络连接。
        /// 保持连接的值为零表示关闭保持连接功能。这意味着，服务端不需要因为客户端不活跃而断开连接。注意：不管保持连接的值是多少，任何时候，只要服务端认为客户端是不活跃或无响应的，可以断开客户端的连接。
        /// </remarks>
        public ushort KeepAlive { get; set; } = 60;
        /// <summary>
        /// 客户端标识符
        /// </summary>
        /// <remarks>
        /// 服务端使用客户端标识符 (ClientId) 识别客户端。连接服务端的每个客户端都有唯一的客户端标识符（ClientId）。客户端和服务端都必须使用ClientId识别两者之间的MQTT会话相关的状态。
        /// 客户端标识符(ClientId) 必须存在而且必须是CONNECT报文有效载荷的第一个字段。
        /// 客户端标识符必须是1.5.3节定义的UTF-8编码字符串。
        /// 服务端必须允许1到23个字节长的UTF-8编码的客户端标识符，客户端标识符只能包含这些字符：“0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ”（大写字母，小写字母和数字）。
        /// 服务端可以允许编码后超过23个字节的客户端标识符(ClientId)。服务端可以允许包含不是上面列表字符的客户端标识符(ClientId)。
        /// 服务端可以允许客户端提供一个零字节的客户端标识符(ClientId) ，如果这样做了，服务端必须将这看作特殊情况并分配唯一的客户端标识符给那个客户端。然后它必须假设客户端提供了那个唯一的客户端标识符，正常处理这个CONNECT报文。
        /// 如果客户端提供了一个零字节的客户端标识符，它必须同时将清理会话标志设置为1。
        /// 如果客户端提供的ClientId为零字节且清理会话标志为0，服务端必须发送返回码为0x02（表示标识符不合格）的CONNACK报文响应客户端的CONNECT报文，然后关闭网络连接。
        /// 如果服务端拒绝了这个ClientId，它必须发送返回码为0x02（表示标识符不合格）的CONNACK报文响应客户端的CONNECT报文，然后关闭网络连接。</remarks>
        public string ClientId { get; set; }
        /// <summary>
        /// 遗嘱主题
        /// </summary>
        /// <remarks>
        /// 如果遗嘱标志被设置为1，有效载荷的下一个字段是遗嘱主题（Will Topic）。
        /// 遗嘱主题必须是 1.5.3节定义的UTF-8编码字符串。
        /// </remarks>
        public string WillTopic { get; set; }
        /// <summary>
        /// 遗嘱消息
        /// </summary>
        /// <remarks>
        /// 如果遗嘱标志被设置为1，有效载荷的下一个字段是遗嘱消息。遗嘱消息定义了将被发布到遗嘱主题的应用消息，见3.1.2.5节的描述。这个字段由一个两字节的长度和遗嘱消息的有效载荷组成，表示为零字节或多个字节序列。长度给出了跟在后面的数据的字节数，不包含长度字段本身占用的两个字节。
        /// 遗嘱消息被发布到遗嘱主题时，它的有效载荷只包含这个字段的数据部分，不包含开头的两个长度字节。
        /// </remarks>
        public byte[] WillMessage { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        private string _UserName;
        /// <summary>
        /// 用户名
        /// </summary>
        /// <remarks>如果用户名（User Name）标志被设置为1，有效载荷的下一个字段就是它。
        /// 用户名必须是 1.5.3节定义的UTF-8编码字符串。
        /// 服务端可以将它用于身份验证和授权。
        /// </remarks>
        public string UserName
        {
            get => this._UserName;
            set
            {
                this.UserNameFlag = value.IsNotNullOrEmpty();
                this._UserName = value;
            }
        }
        /// <summary>
        /// 密码
        /// </summary>
        private string _Password;
        /// <summary>
        /// 密码
        /// </summary>
        /// <remarks>
        /// <para>如果密码（Password）标志被设置为1，有效载荷的下一个字段就是它。</para>
        /// <para>密码字段包含一个两字节的长度字段，长度表示二进制数据的字节数（不包含长度字段本身占用的两个字节），后面跟着0到65535字节的二进制数据。</para>
        /// </remarks>
        public string Password
        {
            get => this._Password;
            set
            {
                this.PasswordFlag = value.IsNotNullOrEmpty();
                this._Password = value;
            }
        }
        /// <summary>
        /// 自定义属性（V5.0新增）
        /// </summary>
        public List<MqttUserProperty> UserProperties { get; set; }
        /// <summary>
        /// 自定义属性（V5.0新增）
        /// </summary>
        public List<MqttUserProperty> WillUserProperties { get; set; }
        /// <summary>
        /// 会话过期时间 以秒为单位（V5.0新增）
        /// </summary>
        /// <remarks>
        /// <para>Type 取值 17(0x11)。Value 是长度为 4 个 byte 的整数，表示以秒为单位的 Session Expiry Interval。如果包含多个 Session Expiry Interval，则认为是 Protocol Error。</para>
        /// <para>如果不存在该字段，或者取值为 0，则 Session 跟随网络连接关闭时结束（MQTT v3.1）。</para>
        /// <para>如果取值为 0xFFFFFFFF(UINT_MAX)，则 Session 永不过期。</para>
        /// <para>如果取值大于 0，Client 和 Server 在网络连接关闭后，必须保存 Session State[MQTT - 3.1.2 - 23]。</para>
        /// <para>参考 4.1 章节获取更多 Session 的信息；参考 4.1.1 获取更多 Session state 的细节信息。</para>
        /// <para>当 Session 超期时，Client 和 Server 不必原子地执行 Session state 的删除。</para>
        /// <para>非规范性描述：</para>
        /// <para>1. 设置 Clean Start 为 1 并且设置 Session Expiry Interval 为 0，等价于 v3.1.1 中设置 CleanSession 为 1；</para>
        /// <para>设置 Clean Start 为 0 且不存在 Session Expriy Interval，等价于 v3.1.1 中 CleanSession 为 0。</para>
        /// <para>2. 对于只在 connected 状态下处理消息的 Client，它会设置 Clean Start 为 1，且 Session Expiry Interval</para>
        /// <para>设置为 0；这样的 Client 在连接到 Server 前不处理已发布的 Application Messages，并且每次都会重新订阅其感兴趣的主题。</para>
        /// <para>3. Client 也许正是使用间歇性可连通网络的服务连接到 Server，这样的 Client 可以设置一个短的</para>
        /// <para>Session Expiry Interval，从而在网络可用时，重新连接到 Server，进行可靠的 Message 投递。如果Client 没有重连，则 Session 超时，Application Message 将会丢失。</para>
        /// <para>4. 当 Client 使用一个较长的 Session Expiry Interval 连接到 Server，它要求 Server 在连接断开时维护Session 的状态一段时间。Client 只有在确定其会在一段时间内会重连到 Server，才许使用一个较长的Session Expiry Interval，如果它将来不会重连到Server，它应该通过DISCONNECT指定Session Expiry Interval 为 0。</para>
        /// <para>5. Client 应该总是通过 CONNACK 中的 Session Present 标志判断 Server 侧是否为其保存了 Session。</para>
        /// <para>6. Client 自身可以不实现 Session expiry，而是使用 Server 返回的 Session Present flag 确定 Session 是否已过期。如果 Cleint 没有实现自己的 Session expriy，它需要在 Session State 中保存 Session State 过期的时间。</para>
        /// </remarks>
        public uint SessionExpiryInterval { get; set; }
        /// <summary>
        /// 接收最大值（V5.0新增）
        /// </summary>
        /// <remarks>
        /// <para>Type 为 33(0x21)，Value 为两字节整数。当存在多个重复的 Receive Maximum 字段或者取值为 0 时，认为是 Protocol Error。</para>
        /// <para>Client 使用该字段通知 Server 其限制并发处理的 QoS 为 1 和 QoS 为 2 的发布的个数。没有机制限制QoS 为 0 的发布。</para>
        /// <para>该字段值只用于当前的网络连接，如果不存在该字段，默认值是 65535。</para>
        /// </remarks>
        public ushort ReceiveMaximum { get; set; }
        /// <summary>
        /// 能够接受的最大封包大小（V5.0新增）
        /// </summary>
        /// <remarks>
        /// <para>Type 为 39(0x27)，Value 为 4byte 整数。该字段表示 Client 能够接受的最大封包大小。如果没有该字段，表示没有限制，限制值来源于控制包中 Remaing Length 以及协议头尺寸能够表示的最大值决定。当该字段出现多次，或取值为 0 时，认为是 Protocol Error。</para>
        /// <para>选择合适的 Maximum Packet Size 是应用自身的责任。</para>
        /// <para> 参考 2.1.4，封包大小是 MQTT 控制包的所有字节数，此字段用于 Client 通知 Server 其能够处理的最大封包尺寸。</para>
        /// <para>Server 端不得发送超过该字段限制的封包[MQTT - 3.1.2 - 24]，如果 Client 收到超过限制的封包，认为是Protocol Error，返回携带 Reason code 为 0x95（Packet too large）的 DISCONNECT 封包。</para>
        /// <para>当分包太大无法发送时，Server 必须丢弃该封包，同时认为已经成功发送了该消息[MQTT - 3.1.2 - 25]。</para>
        /// <para>在 Shared Subscription 场景，有的客户端能够接收，而有的客户端不能接收时，Server 可以只向能够接收的 Client 发送 Message，也可以选择不向任何 Client 发送 Message。</para>
        /// <para>非规范性描述：</para>
        /// <para>如果一个封包由于太大别丢弃，Server 可以将丢弃的封包放入一个“dead letter queue”或者其他诊断性操作。本规范不做进一步要求。</para>
        /// </remarks>
        public uint MaximumPacketSize { get; set; }
        /// <summary>
        /// 接收SERVER发送的主题别名的最大值（V5.0新增）
        /// </summary>
        /// <remarks>
        /// <para>Type 为 34（0x22），取值为两字节整数。如果该字段重复多次出现，认为是 Protocol Error。如果不存在该字段，则默认为 0。</para>
        ///<para>该字段指示 Client 能够接受 Server 发送的主题别名（是一个整数）的最大取值，Client 使用该字段限制一个连接上可以保持主题别名的个数。Server 不得在 PUBLISH 中发送超过此个数的主题别名[MQTT - 3.1.2 - 26]。取值为 0 表示 Client 不接受任何主题别名。如果不存在该字段或该字段取值为 0，Server不得发送任何主题别名给 Client[MQTT - 3.1.2 - 27]。</para>
        /// </remarks>
        public ushort TopicAliasMaximum { get; set; }
        /// <summary>
        /// 请求 Server 通过 CONNACK 返回 Response 信息（V5.0新增）
        /// </summary>
        /// <remarks>
        /// <para>Type 为 25（0x19），取值为一个 Byte，取值范围为 0 或 1。如果该字段重复出现多次，或者取值不是0 或 1，则认为是 Protocol Error。如果不存在该字段，默认值为 0。</para>
        /// <para>lient 使用该字段，请求 Server 通过 CONNACK 返回 Response 信息。取值为 0 时，表示 Server 不能返回 Response 信息[MQTT - 3.1.2 - 28]；如果取值为 1，表示 Server 可以在 CONNACK 中返回 Response 信息。</para>
        /// <para>进一步 Request/Response 交互模型的信息参考 4.10 章节。</para>
        /// </remarks>
        public bool RequestResponseInformation { get; set; }
        /// <summary>
        /// 用该字段指示是否在失败情况下，发送 Reason String 或者 User Properties。（V5.0新增）
        /// </summary>
        /// <remarks>
        /// <para>Type 为 23（0x17），取值为一个 Byte，取值范围为 0 或 1。如果该字段重复出现多次，或者取值不是0 或 1，则认为是 Protocol Error。如果不存在该字段，默认值为 1。</para>
        /// <para>Client使用该字段指示是否在失败情况下，发送 Reason String 或者 User Properties。</para>
        /// <para>如果该字段取值为 0，Server 也许在 CONNACK 或 DISCONNECT 控制包中返回 Reason String 或者Properties，但是不能在处理 PUBLISH，CONNACK 或 DISCONNECT 之外的控制包中发送 Reason String或 Properties[MQTT - 3.1.2 - 29]。如果该字段取值为 0，并且 Client 在除了 PUBLISH，CONNACK 或DISCONNECT之外的控制包中收到了Reason String或User Properties，它应该发送携带Reason Code为0x82（Protocol Error）的 DISCONNECT 控制包。</para>
        /// <para>如果该字段取值为 1，Server 可以在任何允许的控制包中返回 Reason String 或 User Properties。</para>
        /// </remarks>
        public bool RequestProblemInformation { get; set; }
        /// <summary>
        /// 认证方法名称（V5.0新增）
        /// </summary>
        /// <remarks>
        /// <para>Type 为 21（0x15），取值为 UTF-8 编码字符串，表示认证方法名称。如果该字段重复出现多次，则认为是 Protocol Error。如果不存在该字段，不执行扩展认证。</para>
        /// <para>如果 CONNECT 封包中存在该字段，则 Client 在收到 CONNACK 控制包前，不得发送除 AUTH 或 DISCONNECT 之外的其他任何控制包[MQTT - 3.1.2 - 30]。</para>
        /// </remarks>
        public string AuthenticationMethod { get; set; }
        /// <summary>
        /// 认证数据（V5.0新增）
        /// </summary>
        /// <remarks>
        /// <para>Type 为 22（0x16），取值为 Binary Data，包含了认证数据。只存在该字段，而不存在 Authentication Method字段时，认为是 Protocol Error；重复包含该字段认为是 Protocol Error。</para>
        /// <para>该字段取值由 Authentication Method 定义。扩展认证的进一步信息参考 4.12 章节。</para>
        /// </remarks>
        public byte[] AuthenticationData { get; set; }
        /// <summary>
        /// 遗嘱消息延迟间隔 以秒为单位（V5.0新增）
        /// </summary>
        /// <remarks>
        /// <para>该字段 Type 为 24（0x18），取值为 4byte 的整数，表示以秒为单位的遗嘱消息延迟间隔。如果该字段重复出现，则认为是 Protocol Error。如果不存在该字段，默认值为 0，即发布遗嘱消息时没有延迟。</para>
        /// <para>Server 推迟发布 Client 的遗嘱消息，直到 Will Delay Interval 超时或者 Session 结束。如果在 Will Delay Interval 到期前，Client 发起了新的网络连接，则 Server 不能发布该 Client 的遗嘱消息[MQTT - 3.1.3 - 9]。</para>
        /// <para>非规范性描述：</para>
        /// <para>该字段的用途避免在如下场景误发送遗嘱消息：假设 Client 的网络临时断开，随后 Client 重新连接并继续 Session。</para>
        /// <para>如果网络连接使用了现有网络连接的 Client ID，现有连接的遗嘱消息将会发送，除非新的连接的 CleanStart 为 0 并且 Will Delay 大于 0。如果 Will Delay 是 0，遗嘱消息将在现有网络连接关闭时发送；并且如果Clean Start 为 1，应该发送遗嘱消息，因为 Session 已结束。</para>
        /// </remarks>
        public uint WillDelayInterval { get; set; }
        /// <summary>
        /// 编码字符串（V5.0新增）
        /// </summary>
        /// <remarks>
        /// <para>Type 为 3，取值为 UTF-8 编码字符串，用于描述遗嘱消息的内容。如果重复出现该字段，认为是 Protocol Error，取值由发送和接收的应用定义。</para>
        /// </remarks>
        public string WillContentType { get; set; }
        /// <summary>
        /// Payload 格式（V5.0新增）
        /// </summary>
        /// <remarks>
        /// <para>Type 为 1(0x01)，取值为 0x00 或 0x01：</para>
        /// <para>0x00：表示 Will Message 的 Payload 格式是未定义的 Bytes，等价于没有发送 Payload Format Indicator。</para>
        /// <para>0x01：表示 Will Message 的 Payload 格式是 UTF-8 编码字符数据，满足[Unicode] 和 RFC3629 规范该字段重复出现时，认为是 Protocol Error。Server 可以检查 Will Message 是否是该字段指定的格式，如果不是，可以发送携带 Reason Code 为 0x99(Payload format invalid)的 CONNACK 控制包。</para>
        /// </remarks>
        public MqttPayloadFormatIndicator WillPayloadFormatIndicator { get; set; } = MqttPayloadFormatIndicator.Unspecified;
        /// <summary>
        /// 消息超时间隔 以秒为单位（V5.0新增）
        /// </summary>
        /// <remarks>
        /// <para>Type 为 2（0x2），取值为 4byte 的整数，表示以秒为单位的消息超时间隔。如果重复出现，认为是Protocol Error。</para>
        /// <para>如果存在，则取值是遗嘱消息的生存期（以秒为单位），并在服务器发布遗嘱消息时作为“Publication Expiry Interval”发送。如果不存在，当 Server 发布遗嘱消息时，遗嘱消息中不包含 Message Expiry Interval。此处的“Publication Expiry Interval”和“Message Expiry Interval”参考 3.1.3.2.4 章节。</para>
        /// </remarks>
        public uint WillMessageExpiryInterval { get; set; }
        /// <summary>
        /// 用于 Response 消息的 Topic Name（V5.0新增）
        /// </summary>
        /// <remarks>
        /// <para>Type 为 8，取值为 UTF-8 编码的字符串，被用于 Response 消息的 Topic Name。如果该字段重复出现，则认为是 Protocol Error。Reponse Topic 的出现标识遗嘱消息作为 Request。</para>
        /// <para>关于 Request/Response 的交互模型参考 4.10 章节。</para>
        /// </remarks>
        public string WillResponseTopic { get; set; }
        /// <summary>
        /// 用于 Request Message 的发送者去标识对应的 Response Message（V5.0新增）
        /// </summary>
        /// <remarks>
        /// <para>Type 为 9，取值为 Binary Data。被用于 Request Message 的发送者去标识对应的 Response Message。如果该字段重复出现，则认为是 Protocol Error。如果不存在该字段，Requester 不要求任何关联数据。</para>
        /// <para>该字段取值，只对 Request Message 的发送者，和 Response Message 的接收者有意义。</para>
        /// <para>关于 Request/Response 的交互模型参考 4.10 章节。</para>
        /// </remarks>
        public byte[] WillCorrelationData { get; set; }
        /// <summary>
        /// 连接标识保留标识
        /// </summary>
        private bool _ConnectFlagReserved;
        /// <summary>
        /// 连接标识保留标识
        /// </summary>
        public bool ConnectFlagReserved => this._ConnectFlagReserved;
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override bool WriteBuffer(MqttBufferWriter writer)
        {
            if (this.ClientId.IsNullOrEmpty() && !this.CleanSession) this.ClientId = $"EELFMQTT{RandomHelper.GetRandomString(10)}";
            else
            {
                if (this.ClientId.IsNotMatch(@"^[a-z0-9\-_]{1,60}$")) throw new MqttException($"{nameof(this.ClientId)}长度超过了23位.");
            }
            if (this.ProtocolVersion == MqttProtocolVersion.V310)
                this.ProtocolName = "MQIsdp";
            writer.WriteString(this.ProtocolName);
            writer.WriteByte((byte)this.ProtocolVersion);
            var connectFlags = 0b0000_0000;
            if (this.UserNameFlag) connectFlags |= 0b1000_0000;
            if (this.PasswordFlag) connectFlags |= 0b0100_0000;
            if (this.WillTopic.IsNotNullOrEmpty() && this.WillMessage != null && this.WillMessage.Length > 0) this.WillFlag = true;
            if (this.WillFlag) connectFlags |= 0b0000_0100;
            else this.WillRetain = false;
            connectFlags |= ((byte)this.WillQoS << 3) & 0b0001_1000;
            if (this.WillRetain) connectFlags |= 0b0010_0000;
            if (this.CleanSession) connectFlags |= 0b0000_0010;
            writer.WriteByte((byte)connectFlags);
            writer.WriteTwoByteInteger(this.KeepAlive);

            //写属性 V5.0新增
            if (this.ProtocolVersion == MqttProtocolVersion.V500)
            {
                var writerProperty = new MqttBufferWriter();

                writerProperty.WriteProperty(MqttPropertiesId.SessionExpiryInterval, this.SessionExpiryInterval, 4);
                writerProperty.WriteProperty(MqttPropertiesId.AuthenticationMethod, this.AuthenticationMethod);
                writerProperty.WriteProperty(MqttPropertiesId.AuthenticationData, this.AuthenticationData);
                writerProperty.WriteProperty(MqttPropertiesId.RequestProblemInformation, this.RequestProblemInformation);
                writerProperty.WriteProperty(MqttPropertiesId.RequestResponseInformation, this.RequestResponseInformation);
                writerProperty.WriteProperty(MqttPropertiesId.ReceiveMaximum, this.ReceiveMaximum, 2);
                writerProperty.WriteProperty(MqttPropertiesId.TopicAliasMaximum, this.TopicAliasMaximum, 2);
                writerProperty.WriteProperty(MqttPropertiesId.MaximumPacketSize, this.MaximumPacketSize, 4);
                writerProperty.WriteUserProperties(this.UserProperties);
                writer.Write(writerProperty);
                writerProperty.Dispose();
            }

            writer.WriteString(this.ClientId);

            if (this.WillFlag)
            {
                //写用户属性 V5.0新增
                if (this.ProtocolVersion == MqttProtocolVersion.V500)
                {
                    var writerProperty = new MqttBufferWriter();

                    writerProperty.WriteProperty(MqttPropertiesId.PayloadFormatIndicator, (byte)this.WillPayloadFormatIndicator);
                    writerProperty.WriteProperty(MqttPropertiesId.MessageExpiryInterval, this.WillMessageExpiryInterval, 4);
                    writerProperty.WriteProperty(MqttPropertiesId.ResponseTopic, this.WillResponseTopic);
                    writerProperty.WriteProperty(MqttPropertiesId.CorrelationData, this.WillCorrelationData);
                    writerProperty.WriteProperty(MqttPropertiesId.ContentType, this.WillContentType);
                    writerProperty.WriteUserProperties(this.WillUserProperties);
                    writerProperty.WriteProperty(MqttPropertiesId.WillDelayInterval, this.WillDelayInterval, 4);

                    writer.Write(writerProperty);
                    writerProperty.Dispose();
                }

                writer.WriteString(this.WillTopic);
                writer.WriteBinaryData(this.WillMessage);
            }
            if (this.UserName.IsNotNullOrEmpty())
            {
                writer.WriteString(this.UserName);
                if (this.Password.IsNotNullOrEmpty())
                    writer.WriteString(this.Password);
            }
            return true;
        }
        ///<inheritdoc/>
        public override bool ReadBuffer(MqttBufferReader reader)
        {
            if (this.PacketType != PacketType.CONNECT)
            {
                this.SetError(ReasonCode.MALFORMED_PACKET, $"无效报文.");
                return false;
            }
            //协议名称
            this.ProtocolName = reader.ReadString();
            //协议等级
            this.ProtocolVersion = (MqttProtocolVersion)reader.ReadByte();

            if (this.ProtocolVersion == 0)
            {
                this.SetError(ReasonCode.UNSUPPORTED_PROTOCOL_VERSION, "协议版本不支持.");
                return false;
            }

            if (this.ProtocolName != "MQTT" && (int)this.ProtocolVersion > 3 || (this.ProtocolName != "MQIsdp" && this.ProtocolVersion == MqttProtocolVersion.V310))
            {
                this.SetError(ReasonCode.PROTOCOL_ERROR, "MQTT协议名称与MQTT版本不匹配.");
                //throw new MqttException("MQTT协议名称与MQTT 版本不匹配.");
                return false;
            }

            var connectFlags = reader.ReadByte();

            this.UserNameFlag = (connectFlags & 0b1000_0000) > 0;
            this.PasswordFlag = (connectFlags & 0b0100_0000) > 0;
            this.CleanSession = (connectFlags & 0b0000_0010) > 0;

            var willMessageFlag = (connectFlags & 0b0000_0100) > 0;
            var willMessageQos = (connectFlags >> 3) & 3;
            var retainFlag = (connectFlags & 0b0010_0000) > 0;
            this._ConnectFlagReserved = (connectFlags & 0b0000_0001) == 1;
            if (willMessageFlag)
            {
                this.WillFlag = true;
            }
            this.WillQoS = (QualityOfServiceLevel)willMessageQos;
            this.WillRetain = retainFlag;
            this.KeepAlive = reader.ReadTwoByteInteger();
            this.UserProperties = new List<MqttUserProperty>();
            this.WillUserProperties = new List<MqttUserProperty>();

            if (this.ProtocolVersion == MqttProtocolVersion.V500)
            {
                var length = reader.ReadVariableByteInteger();
                var endLength = reader.Position + length;
                if (length > 0)
                {
                    while (!reader.EndOfStream && reader.Position < endLength)
                    {
                        var id = reader.ReadByte();
                        switch ((MqttPropertiesId)id)
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
                            case MqttPropertiesId.RequestProblemInformation:
                                this.RequestProblemInformation = reader.ReadByte() == 1;
                                break;
                            case MqttPropertiesId.RequestResponseInformation:
                                this.RequestResponseInformation = reader.ReadByte() == 1;
                                break;
                            case MqttPropertiesId.None:

                                break;
                            default:
                                this.SetError(ReasonCode.PROTOCOL_ERROR, $"MQTT Protocol Error: {id}");
                                return false;
                                //throw new MqttProtocolException(string.Format("MQTT Protocol Error: {0}", id));
                        }
                    }
                }
            }
            this.ClientId = reader.ReadString();
            if (this.WillFlag)
            {
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
                                case MqttPropertiesId.UserProperty:
                                    this.WillUserProperties.Add(new MqttUserProperty(reader.ReadString(), reader.ReadString()));
                                    break;
                                case MqttPropertiesId.PayloadFormatIndicator:
                                    this.WillPayloadFormatIndicator = (MqttPayloadFormatIndicator)reader.ReadByte();
                                    break;
                                case MqttPropertiesId.MessageExpiryInterval:
                                    this.WillMessageExpiryInterval = reader.ReadFourByteInteger();
                                    break;
                                case MqttPropertiesId.ResponseTopic:
                                    this.WillResponseTopic = reader.ReadString();
                                    break;
                                case MqttPropertiesId.CorrelationData:
                                    this.WillCorrelationData = reader.ReadBinaryData();
                                    break;
                                case MqttPropertiesId.ContentType:
                                    this.WillContentType = reader.ReadString();
                                    break;
                                case MqttPropertiesId.WillDelayInterval:
                                    this.WillDelayInterval = reader.ReadFourByteInteger();
                                    break;
                                default:
                                    this.SetError(ReasonCode.PROTOCOL_ERROR, $"MQTT Protocol Error: {id}");
                                    return false;
                                    //throw new MqttProtocolException($"MQTT Protocol Error: {id}");
                            }
                        }
                    }
                }
                this.WillTopic = reader.ReadString();
                this.WillMessage = reader.ReadBinaryData();
            }
            if (this.UserNameFlag)
                this.UserName = reader.ReadString();
            if (this.PasswordFlag)
                this.Password = reader.ReadString();
            return true;
        }
        /// <summary>
        /// 重写转字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.PacketType}: [ClientId={this.ClientId}] [UserName={this.UserName}] [Password={(this.Password.IsNullOrEmpty() ? string.Empty : "******")}] [KeepAlive={this.KeepAlive}] [CleanSession={this.CleanSession}] [Qos={(int)this.WillQoS}] [WillTopic={this.WillTopic}] [SessionExpiryInterval={this.SessionExpiryInterval}] [ReceiveMaximum={this.ReceiveMaximum}] [MaximumPacketSize={this.MaximumPacketSize}] [TopicAliasMaximum={this.TopicAliasMaximum}] [RequestResponseInformation={(this.RequestResponseInformation ? 1 : 0)}] [RequestProblemInformation={(this.RequestProblemInformation ? 1 : 0)}] [AuthenticationMethod={this.AuthenticationMethod}] [WillDelayInterval={this.WillDelayInterval}] [WillContentType={this.WillContentType}] [WillPayloadFormatIndicator={(int)this.WillPayloadFormatIndicator}] [WillMessageExpiryInterval={this.WillMessageExpiryInterval}] [WillResponseTopic={this.WillResponseTopic}]{(this.PacketStatus == PacketStatus.Error ? $" [ErrorCode={this.ErrorCode}] [ErrorMessage={this.ErrorMessage}]" : "")}";
        }
        #endregion
    }
}