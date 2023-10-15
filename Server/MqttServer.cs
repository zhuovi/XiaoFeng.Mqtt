using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using XiaoFeng.Mqtt;
using XiaoFeng.Net;
using XiaoFeng.Log;
using XiaoFeng.Mqtt.Packets;
using XiaoFeng.Mqtt.Internal;
using System.Net.Sockets;
using XiaoFeng.FTP;
using System.Collections.Concurrent;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-27 13:58:11                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Server
{
    /// <summary>
    /// MQTT服务端
    /// </summary>
    public class MqttServer : Disposable, IMqttServer
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MqttServer() { }
        /// <summary>
        /// 设置监听地址
        /// </summary>
        /// <param name="endPoint">监听地址</param>
        public MqttServer(IPEndPoint endPoint)
        {
            this.EndPoint = endPoint;
        }
        /// <summary>
        /// 监听地址
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="port">端口</param>
        public MqttServer(string host, int port)
        {
            if (port <= 0) port = 1883;
            this.EndPoint = new IPEndPoint(host.IsNullOrEmpty() ? IPAddress.Any : IPAddress.Parse(host), port);
        }
        /// <summary>
        /// 设置监听端口
        /// </summary>
        /// <param name="port">端口</param>
        public MqttServer(int port)
        {
            this.EndPoint = new IPEndPoint(IPAddress.Any, port);
        }
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="options">配置</param>
        public MqttServer(MqttServerOptions options)
        {
            this._ServerOptions = options;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 终节点
        /// </summary>
        private IPEndPoint EndPoint { get; set; }
        /// <summary>
        /// 服务端
        /// </summary>
        private ISocketServer Server { get; set; }
        /// <summary>
        /// 配置
        /// </summary>
        private MqttServerOptions _ServerOptions;
        /// <summary>
        /// 配置
        /// </summary>
        public MqttServerOptions ServerOptions => this._ServerOptions;
        /// <summary>
        /// MQTT服务器凭证
        /// </summary>
        private ConcurrentDictionary<string, IMqttServerCredential> MqttServerCredentials { get; set; }
        /// <summary>
        /// MQTT服务端主题消息
        /// </summary>
        public ConcurrentDictionary<string, IMqttServerTopicMessage> MqttServerTopicMessages { get; set; }
        /// <summary>
        /// MQTT服务器凭证
        /// </summary>
        public ICollection<IMqttServerCredential> Credentials=> this.MqttServerCredentials?.Values;
        #endregion

        #region 事件
        /// <summary>
        /// 启动事件
        /// </summary>
        public event MqttServerStaredEventHandler OnStarted;
        /// <summary>
        /// 停止事件
        /// </summary>
        public event MqttServerStopedEventHandler OnStoped;
        /// <summary>
        /// 新连接事件
        /// </summary>
        public event MqttServerConnectedEventHandler OnConnected;
        /// <summary>
        /// 断开事件
        /// </summary>
        public event MqttServerDisconnectedEventHandler OnDisconnected;
        /// <summary>
        /// 服务端出错事件
        /// </summary>
        public event MqttServerErrorEventHandler OnError;
        /// <summary>
        /// 客户端出错事件
        /// </summary>
        public event MqttServerClientErrorEventHandler OnClientError;
        /// <summary>
        /// 接收消息事件
        /// </summary>
        public event MqttServerMessageEventHandler OnMessage;
        #endregion

        #region 方法

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            if (this.ServerOptions == null) this._ServerOptions = new MqttServerOptions();
            if (this.EndPoint != null) this.ServerOptions.EndPoint = this.EndPoint;
            if (this.MqttServerCredentials == null)
                this.MqttServerCredentials = new ConcurrentDictionary<string, IMqttServerCredential>();
            if (this.MqttServerTopicMessages == null)
                this.MqttServerTopicMessages = new ConcurrentDictionary<string, IMqttServerTopicMessage>();
            if (this.Server != null) return;
            this.Server = new SocketServer()
            {
                ConnectTimeout = this.ServerOptions.ConnectTimeout * 1000,
                ReceiveTimeout = this.ServerOptions.ReadTimout,
                SendTimeout = this.ServerOptions.WriteTimeout,
                ReceiveBufferSize = this.ServerOptions.ReadBufferSize,
                SendBufferSize = this.ServerOptions.WriteBufferSize,
                EndPoint = this.ServerOptions.EndPoint
            };
            if (this.ServerOptions.UseTls)
            {
                this.Server.SslProtocols = this.ServerOptions.SslProtocols;
                this.Server.Certificate = this.ServerOptions.Certificate;
            }
            this.Server.OnStart += (o, e) =>
            {
                this.OnStarted?.Invoke(this);
            };
            this.Server.OnStop += (o, e) =>
            {
                this.OnStoped?.Invoke(this);
            };
            this.Server.OnClientError += (c, ep, e) =>
            {
                this.OnClientError?.Invoke(c, ep, e.Message);
            };
            this.Server.OnDisconnected += (c, e) =>
            {
                this.OnDisconnected?.Invoke(c);
            };
            this.Server.OnError += (o, e) =>
            {
                this.OnError?.Invoke(this, e.Message);
            };
            this.Server.OnNewConnection += (c, e) =>
            {
                c.InitClientData();
                this.OnConnected?.Invoke(c);
            };
            this.Server.OnMessageByte += (c, m, e) =>
            {
                this.ReceiveMessage(c, m);
            };
        }
        #endregion

        #region 启动
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            this.Init();
            if (!this.Server.Active)
                this.Server.Start();
        }
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="options">配置</param>
        public void Start(MqttServerOptions options)
        {
            this._ServerOptions = options;
            this.Start();
        }
        #endregion

        #region 停止
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            this.Server.Stop();
            this.Server = null;
        }
        #endregion

        #region 接收数据
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="bytes">字节流</param>
        private async void ReceiveMessage(ISocketClient client, byte[] bytes)
        {
            var packetType = (PacketType)(bytes[0] >> 4);
            var ClientData = client.GetClientData();
            var ProtocolVersion = MqttProtocolVersion.Unknown;
            if (packetType > 0 && ClientData.ConnectPacket != null)
                ProtocolVersion = ClientData.ConnectPacket.ProtocolVersion;
            ResultPacket result = null;
            var clientId = ClientData.ConnectPacket?.ClientId;
            switch (packetType)
            {
                case PacketType.CONNECT:
                    var connPacket = new ConnectPacket(bytes, ProtocolVersion);
                    OnMessage?.Invoke(client, new ResultPacket(ResultType.Success, $"New client connected from {client.EndPoint} as {connPacket.ClientId} ({connPacket})."));
                    var connActResult = await this.ConnActAsync(client, connPacket).ConfigureAwait(false);
                    if (connActResult.ResultType == ResultType.Error)
                    {
                        client.Stop();
                        OnError?.Invoke(this, connActResult.Message);
                        return;
                    }
                    else
                    {
                        if (connPacket.WillMessage == null || connPacket.WillMessage.Length == 0)
                            OnMessage?.Invoke(client, new ResultPacket(ResultType.Success, "No WillMessage specified."));
                        result = new ResultPacket(connActResult.MqttPacket, ResultType.Success, $"Sending CONNACK to {connPacket.ClientId} ({connActResult.MqttPacket}).");
                        ClientData.ConnectPacket = connPacket;
                    }
                    break;
                case PacketType.AUTH:
                    var authPacket = new AuthPacket(bytes, ProtocolVersion);
                    OnMessage?.Invoke(client, new ResultPacket(ResultType.Success, $"Received AUTH from {client.EndPoint} as {clientId} ({authPacket})."));
                    var processAuthResult = await ProcessAuthPacketAsync(client, authPacket).ConfigureAwait(false);
                    if (processAuthResult.ResultType == ResultType.Error)
                    {
                        OnError?.Invoke(this, processAuthResult.Message);
                    }
                    break;
                case PacketType.DISCONNECT:
                    var disconnPacket = new DisconnectPacket(bytes, ProtocolVersion);
                    OnMessage?.Invoke(client, new ResultPacket(ResultType.Success, $"Received DISCONNECT from {client.EndPoint} as {clientId} ({disconnPacket})."));
                    var processDisconnResult = await ProcessDisconnectAsync(client, disconnPacket).ConfigureAwait(false);
                    if (processDisconnResult.ResultType == ResultType.Error)
                    {
                        OnError?.Invoke(this, processDisconnResult.Message);
                    }
                    else
                    {
                        client.Stop();
                    }
                    return;
                case PacketType.PINGREQ:
                    var pingReqPacket = new PingReqPacket(bytes, ProtocolVersion);
                    OnMessage?.Invoke(client, new ResultPacket(ResultType.Success, $"Received PINGREQ from {client.EndPoint} as {clientId} ({pingReqPacket})."));
                    var pingRespResult = await PingRespAsync(client, pingReqPacket).ConfigureAwait(false);
                    if (pingRespResult.ResultType == ResultType.Error)
                    {
                        OnError?.Invoke(this, pingRespResult.Message);
                        return;
                    }
                    else
                    {
                        result = new ResultPacket(pingRespResult.MqttPacket, ResultType.Success, $"Sending PINGRESP to {clientId} ({pingRespResult.MqttPacket}).");
                    }
                    break;
                case PacketType.PUBLISH:
                    var publishPacket = new PublishPacket(bytes, ProtocolVersion);
                    OnMessage?.Invoke(client, new ResultPacket(ResultType.Success, $"Received PUBLISH from {client.EndPoint} as {clientId} ({publishPacket})."));
                    var publishResult = await PubAckAsync(client, publishPacket).ConfigureAwait(false);
                    if (publishResult.ResultType == ResultType.Error)
                    {
                        OnError?.Invoke(this, publishResult.Message);
                        return;
                    }
                    else
                    {
                        result = new ResultPacket(publishResult.MqttPacket, ResultType.Success, $"Sending {publishResult.MqttPacket.PacketType.ToString().ToUpper()} to {clientId} ({publishResult.MqttPacket}).");
                    }
                    break;
                case PacketType.PUBREL:
                    var RelPacket = new PubRelPacket(bytes, ProtocolVersion);
                    OnMessage?.Invoke(client, new ResultPacket(ResultType.Success, $"Received {RelPacket.PacketType} from {client.EndPoint} as {clientId} ({RelPacket})."));
                    var pubcompResult = await PubCompAsync(client, RelPacket).ConfigureAwait(false);
                    if (pubcompResult.ResultType == ResultType.Error)
                    {
                        OnError?.Invoke(this, pubcompResult.Message);
                        return;
                    }
                    else
                    {
                        result = new ResultPacket(pubcompResult.MqttPacket, ResultType.Success, $"Sending PUBCOMP to {clientId} ({pubcompResult.MqttPacket}).");
                    }
                    break;
                case PacketType.SUBSCRIBE:
                    var subPacket = new SubscribePacket(bytes, ProtocolVersion);
                    OnMessage?.Invoke(client, new ResultPacket(ResultType.Success, $"Received {subPacket.PacketType} from {client.EndPoint} as {clientId} ({subPacket})."));
                    var subActResult = await this.SubActAsync(client, subPacket).ConfigureAwait(false);
                    if (subActResult.ResultType == ResultType.Error)
                    {
                        OnError?.Invoke(this, subActResult.Message);
                        return;
                    }
                    else
                    {
                        result = new ResultPacket(subActResult.MqttPacket, ResultType.Success, $"Sending SUBACK to {clientId} ({subActResult.MqttPacket}).");
                    }
                    break;
                case PacketType.UNSUBSCRIBE:
                    var unsubPacket = new UnsubscribePacket(bytes, ProtocolVersion);
                    OnMessage?.Invoke(client, new ResultPacket(ResultType.Success, $"Received  {unsubPacket.PacketType}  from {client.EndPoint} as {clientId} ({unsubPacket})."));
                    var unsubActResult = await this.UnsubAckAsync(client, unsubPacket).ConfigureAwait(false);
                    if (unsubActResult.ResultType == ResultType.Error)
                    {
                        OnError?.Invoke(this, unsubActResult.Message);
                        return;
                    }
                    else
                    {
                        result = new ResultPacket(unsubActResult.MqttPacket, ResultType.Success, $"Sending UNSUBACK to {clientId} ({unsubActResult.MqttPacket}).");
                    }
                    break;
                default:
                    break;
            }
            if (result != null && result.MqttPacket?.PacketType > 0)
                this.OnMessage?.Invoke(client, result);
        }
        #endregion

        #region 是否存在ClientId
        /// <summary>
        /// 是否存在ClientId
        /// </summary>
        /// <param name="clientId">客户端Id</param>
        /// <returns></returns>
        public bool Contains(string clientId)
        {
            return this.Server.Clients != null && this.Server.Clients.Count > 0 && this.Server.Clients.LongCount(c => c.GetClientData().ConnectPacket != null && c.GetClientData().ConnectPacket.ClientId == clientId) > 0;
        }
        #endregion

        #region 发送连接应答
        /// <summary>
        /// 发送连接应答
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">连接包</param>
        /// <returns></returns>
        public async Task<ResultPacket> ConnActAsync(ISocketClient client, ConnectPacket packet)
        {
            var result = new ResultPacket(packet, ResultType.Error, "");
            var AckPacket = new ConnectActPacket();
            if ((int)packet.ProtocolVersion <= 0 || (int)packet.ProtocolVersion > 5)
            {
                AckPacket.ReasonCode = ReasonCode.UNSUPPORTED_PROTOCOL_VERSION;
                AckPacket.ReasonString = result.Message = $"Connection Refused: Unsupported Protocol Version [{packet.ProtocolVersion}] ";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if ((packet.ProtocolVersion == MqttProtocolVersion.V310 && packet.ProtocolName != "MQIsdp") || ((int)packet.ProtocolVersion > 3 && packet.ProtocolName != "MQTT"))
            {
                AckPacket.ReasonCode = ReasonCode.UNSUPPORTED_PROTOCOL_VERSION;
                AckPacket.ReasonString = result.Message = $"Connection Refused: Unsupported Protocol Version [{packet.ProtocolName}] ";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (packet.ConnectFlagReserved)
            {
                AckPacket.ReasonCode = ReasonCode.MALFORMED_PACKET;
                AckPacket.ReasonString = result.Message = $"Connection Refused: Reserved Flag is not 0";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (!packet.WillFlag && packet.WillRetain)
            {
                AckPacket.ReasonCode = ReasonCode.MALFORMED_PACKET;
                AckPacket.ReasonString = result.Message = $"Connection Refused: Will Retain is not 0";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }

            if (packet.WillRetain && packet.WillMessage != null && packet.WillMessage.Length > 0 && !this.ServerOptions.RetainAvailable)
            {
                AckPacket.ReasonCode = ReasonCode.RETAIN_NOT_SUPPORTED;
                AckPacket.ReasonString = result.Message = $"Connection Refused: Retain not supported";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }

            if (packet.ClientId.IsNullOrEmpty())
            {
                AckPacket.ReasonCode = ReasonCode.CLIENT_IDENTIFIER_NOT_VALID;
                AckPacket.ReasonString = result.Message = $"Connection Refused: ClientId is null or empty";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (this.Contains(packet.ClientId))
            {
                AckPacket.ReasonCode = ReasonCode.CLIENT_IDENTIFIER_NOT_VALID;
                AckPacket.ReasonString = result.Message = $"Connection Refused: ClientId [{packet.ClientId}] is already connected";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }

            AckPacket.AssignedClientIdentifier = packet.ClientId;

            if (packet.WillFlag && MqttHelper.IsValidTopicName(packet.WillTopic))
            {
                AckPacket.ReasonCode = ReasonCode.UNSPECIFIED_ERROR;
                AckPacket.ReasonString = result.Message = $"Client WillTopic [{packet.WillTopic}] verification failed";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (packet.WillFlag && (packet.WillMessage == null || packet.WillMessage.Length == 0 || packet.WillMessage.Length > 65535))
            {
                AckPacket.ReasonCode = ReasonCode.UNSPECIFIED_ERROR;
                AckPacket.ReasonString = result.Message = $"Client WillMessage [{packet.WillMessage}] verification failed";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (packet.ProtocolVersion == MqttProtocolVersion.V500 && packet.WillFlag && (packet.WillUserProperties == null || packet.WillUserProperties.Count == 0))
            {
                AckPacket.ReasonCode = ReasonCode.UNSPECIFIED_ERROR;
                AckPacket.ReasonString = result.Message = $"Client WillUserProperties [{packet.WillUserProperties.ToJson()}] verification failed";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (packet.UserNameFlag && packet.UserName.Length > 65535)
            {
                AckPacket.ReasonCode = ReasonCode.BAD_USERNAME_OR_PASSWORD;
                AckPacket.ReasonString = result.Message = $"Client UserName [{packet.UserName}] verification failed";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (packet.PasswordFlag && packet.Password.Length > 65535)
            {
                AckPacket.ReasonCode = ReasonCode.BAD_USERNAME_OR_PASSWORD;
                AckPacket.ReasonString = result.Message = $"Client Password [{packet.Password}] verification failed";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (packet.UserNameFlag && packet.UserName.IsNotNullOrEmpty() && packet.PasswordFlag && packet.Password.IsNullOrEmpty())
            {
                AckPacket.ReasonCode = ReasonCode.BAD_USERNAME_OR_PASSWORD;
                AckPacket.ReasonString = result.Message = $"Client Password [{packet.Password}] verification failed";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (packet.UserNameFlag && packet.UserName.IsNullOrEmpty() && packet.PasswordFlag && packet.Password.IsNotNullOrEmpty())
            {
                AckPacket.ReasonCode = ReasonCode.BAD_USERNAME_OR_PASSWORD;
                AckPacket.ReasonString = result.Message = $"Client UserName [{packet.UserName}] verification failed";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (packet.UserNameFlag && packet.UserName.IsNotNullOrEmpty() && packet.UserName.IsNotMatch(@"^[a-z0-9\-_]{1,23}$"))
            {
                AckPacket.ReasonCode = ReasonCode.BAD_USERNAME_OR_PASSWORD;
                AckPacket.ReasonString = result.Message = $"Client UserName [{packet.UserName}] verification failed";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (packet.PasswordFlag && packet.Password.IsNotNullOrEmpty() && packet.Password.IsNotMatch(@"^[a-z0-9\-_]{1,23}$"))
            {
                AckPacket.ReasonCode = ReasonCode.BAD_USERNAME_OR_PASSWORD;
                AckPacket.ReasonString = result.Message = $"Client Password [{packet.Password}] verification failed";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (packet.WillFlag && packet.WillTopic.IsNotNullOrEmpty() && !MqttHelper.IsValidTopicName(packet.WillTopic))
            {
                result.Message = $"Client WillTopic [{packet.WillTopic}] verification failed";
                return result;
            }

            if (!this.ServerOptions.AllowAnonymousAccess && (!packet.UserNameFlag || !packet.PasswordFlag))
            {
                var DisconnPacket = new DisconnectPacket()
                {
                    ReasonCode = ReasonCode.BANNED,
                    ReasonString = "Authentication failed: The server does not allow anonymous access"
                };
                result.MqttPacket = DisconnPacket;
                await DisconnectAsync(client, DisconnPacket).ConfigureAwait(false);
                OnMessage?.Invoke(client, new ResultPacket(DisconnPacket, $"Sending {DisconnPacket.PacketType} to {client.EndPoint}"));
                result.Message = DisconnPacket.ReasonString;
                return result;
            }
            if (packet.UserNameFlag && packet.UserName.IsNotNullOrEmpty())
            {
                var DisconnPacket = new DisconnectPacket()
                {
                    ReasonCode = ReasonCode.BAD_USERNAME_OR_PASSWORD,
                    ReasonString = "Authentication failed: Account or password error"
                };
                if (this.MqttServerCredentials != null && this.MqttServerCredentials.Count > 0 && this.MqttServerCredentials.TryGetValue(packet.UserName, out var credential))
                {
                    if (packet.Password == credential.Password)
                    {
                        if (credential.AllowClientIp == null || credential.AllowClientIp.Count == 0)
                        {
                            DisconnPacket.ReasonCode = ReasonCode.SUCCESS;
                        }
                        else
                        {
                            if (credential.AllowClientIp.Contains(client.EndPoint.Address.ToString()))
                                DisconnPacket.ReasonCode = ReasonCode.SUCCESS;
                            else
                            {
                                DisconnPacket.ReasonCode = ReasonCode.NOT_AUTHORIZED;
                                DisconnPacket.ReasonString = "Authentication failed: IP address is not allowed";
                            }
                        }
                    }
                }
                if (DisconnPacket.ReasonCode != ReasonCode.SUCCESS)
                {
                    result.MqttPacket = DisconnPacket;
                    await DisconnectAsync(client, DisconnPacket).ConfigureAwait(false);
                    OnMessage?.Invoke(client, new ResultPacket(DisconnPacket,ResultType.Success, $"Sending {DisconnPacket.PacketType} to {client.EndPoint}"));
                    result.Message = DisconnPacket.ReasonString;
                    return result;
                }
            }

            if (this.ServerOptions.ServerReference.IsNotNullOrEmpty())
            {
                AckPacket.ServerReference = this.ServerOptions.ServerReference;
                AckPacket.ReasonCode = ReasonCode.SERVER_MOVED;
            }

            AckPacket.RetainAvailable = this.ServerOptions.RetainAvailable;
            AckPacket.MaximumPacketSize = this.ServerOptions.MaximumPacketSize;
            AckPacket.MaximumQoS = this.ServerOptions.MaximumQoS;
            AckPacket.ReceiveMaximum = this.ServerOptions.ReceiveMaximum;

            if (packet.KeepAlive > 0 && this.ServerOptions.ServerKeepAlive <= 0)
                this.ServerOptions.ServerKeepAlive = packet.KeepAlive;

            AckPacket.ServerKeepAlive = this.ServerOptions.ServerKeepAlive;

            if (packet.SessionExpiryInterval > 0 && this.ServerOptions.SessionExpiryInterval <= 0)
                this.ServerOptions.SessionExpiryInterval = packet.SessionExpiryInterval;

            AckPacket.SessionExpiryInterval = this.ServerOptions.SessionExpiryInterval;
            AckPacket.SharedSubscriptionAvailable = this.ServerOptions.SharedSubscriptionAvailable;
            AckPacket.SubscriptionIdentifiersAvailable = this.ServerOptions.SubscriptionIdentifiersAvailable;
            AckPacket.ReasonCode = ReasonCode.SUCCESS;
            result.ResultType = ResultType.Success;
            result.MqttPacket = AckPacket;
            if (!await this.SendAsync(AckPacket, client).ConfigureAwait(false)) result.ResultType = ResultType.Error;
            return result;
        }
        #endregion

        #region 发送订阅应答
        /// <summary>
        /// 获取指定长度的ReasonCode集合
        /// </summary>
        /// <param name="reasonCode">原因码</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        private List<ReasonCode> GetReasonCodes(ReasonCode reasonCode, int length)
        {
            var list = new List<ReasonCode>();
            for (var i = 0; i < length; i++)
                list.Add(reasonCode);
            return list;
        }
        /// <summary>
        /// 发送订阅应答
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">订阅包</param>
        /// <returns></returns>
        public async Task<ResultPacket> SubActAsync(ISocketClient client, SubscribePacket packet)
        {
            var IsConnected = await this.IsConnectedAsync(client).ConfigureAwait(false);
            if (IsConnected.ResultType != ResultType.Success) return IsConnected;

            var result = new ResultPacket(packet, ResultType.Error, "");
            var AckPacket = new SubAckPacket();

            if (packet.FixedFlags != 0x02)
            {
                AckPacket.ReasonCodes = this.GetReasonCodes(ReasonCode.MALFORMED_PACKET, packet.TopicFilters.Count);
                AckPacket.ReasonString = result.Message = $"SUBSCRIBE FixedFlags is not 0x02";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (packet.PacketIdentifier == 0)
            {
                AckPacket.ReasonCodes = this.GetReasonCodes(ReasonCode.PACKET_IDENTIFIER_NOT_FOUND, packet.TopicFilters.Count);
                AckPacket.ReasonString = result.Message = $"SUBSCRIBE PacketIdentifier is null or empty";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (packet.PacketIdentifier > 0xffff)
            {
                AckPacket.ReasonCodes = this.GetReasonCodes(ReasonCode.PROTOCOL_ERROR, packet.TopicFilters.Count);
                AckPacket.ReasonString = result.Message = $"SUBSCRIBE PacketIdentifier is too large";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            /*
            if (packet.SubscriptionIdentifier == 0)
            {
                AckPacket.ReasonCodes = this.GetReasonCodes(ReasonCode.SUBCRIPTION_IDENTIFIER_NOT_SUPPORTED, packet.TopicFilters.Count);
                AckPacket.ReasonString = result.Message = $"SUBSCRIBE SubscriptionIdentifier is null or empty";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (packet.SubscriptionIdentifier > 0xfffffff)
            {
                AckPacket.ReasonCodes = this.GetReasonCodes(ReasonCode.SUBCRIPTION_IDENTIFIER_NOT_SUPPORTED, packet.TopicFilters.Count);
                AckPacket.ReasonString = result.Message = $"SUBSCRIBE SubscriptionIdentifier is too large";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }*/
            if (packet.TopicFilters == null || packet.TopicFilters.Count == 0)
            {
                AckPacket.ReasonCodes = this.GetReasonCodes(ReasonCode.PROTOCOL_ERROR, packet.TopicFilters.Count);
                AckPacket.ReasonString = result.Message = $"SUBSCRIBE TopicFilters is null or empty";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (packet.TopicFilters.Any(t => t.Topic.IsNullOrEmpty() || !MqttHelper.IsValidTopicFilter(t.Topic)))
            {
                AckPacket.ReasonCodes = this.GetReasonCodes(ReasonCode.PROTOCOL_ERROR, packet.TopicFilters.Count); AckPacket.ReasonString = result.Message = $"SUBSCRIBE TopicName is invalid";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            var ClientData = client.GetClientData();
            if (ClientData.TopicFilters == null)
                ClientData.TopicFilters = new List<TopicFilter>();
            var reasonCodes = new List<ReasonCode>();
            var AddTopicFilters = new List<TopicFilter>();
            var index = 0;
            do
            {
                var t = packet.TopicFilters[index++];
                if (MqttHelper.IsValidTopicFilter(t.Topic))
                {
                    var anyIndex = ClientData.TopicFilters.FindIndex(a => a.Topic == t.Topic);
                    if (anyIndex > -1)
                    {
                        ClientData.TopicFilters[anyIndex] = t;
                        reasonCodes.Add(ReasonCode.SUCCESS);
                    }
                    else
                    {
                        ClientData.TopicFilters.Add(t);
                        AddTopicFilters.Add(t);
                        reasonCodes.Add(ReasonCode.SUCCESS);
                    }
                }
                else
                {
                    reasonCodes.Add(ReasonCode.TOPIC_NAME_INVALID);
                }
            } while (index < packet.TopicFilters.Count);

            AckPacket.ReasonCodes = reasonCodes;
            AckPacket.PacketIdentifier = packet.PacketIdentifier;
            AckPacket.UserProperties = packet.UserProperties;
            result.ResultType = ResultType.Success;
            result.MqttPacket = AckPacket;
            if (!await this.SendAsync(AckPacket, client).ConfigureAwait(false)) result.ResultType = ResultType.Error;
            if(result.ResultType == ResultType.Success && AddTopicFilters.Count>0)
            {
                AddTopicFilters.Each(t =>
                {
                    this.SendPublishAsync(client, t).ConfigureAwait(false).GetAwaiter();
                });
            }
            return result;
        }
        #endregion

        #region 发送取消订阅应答
        /// <summary>
        /// 发送取消订阅应答
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">取消订阅包</param>
        /// <returns></returns>
        public async Task<ResultPacket> UnsubAckAsync(ISocketClient client, UnsubscribePacket packet)
        {
            var IsConnected = await this.IsConnectedAsync(client).ConfigureAwait(false);
            if (IsConnected.ResultType != ResultType.Success) return IsConnected;

            var result = new ResultPacket(packet, ResultType.Error, "");
            var AckPacket = new UnsubAckPacket();

            if (packet.FixedFlags != 0x02)
            {
                AckPacket.ReasonCodes = this.GetReasonCodes(ReasonCode.MALFORMED_PACKET, packet.TopicFilters.Count);
                AckPacket.ReasonString = result.Message = $"UNSUBSCRIBE FixedFlags is not 0x02";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }

            if (packet.TopicFilters == null || packet.TopicFilters.Count == 0)
            {
                AckPacket.ReasonCodes = this.GetReasonCodes(ReasonCode.PROTOCOL_ERROR, packet.TopicFilters.Count);
                AckPacket.ReasonString = result.Message = $"UNSUBSCRIBE TopicFilters is null or empty";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (packet.TopicFilters.Any(t => t.Topic.IsNullOrEmpty() || !MqttHelper.IsValidTopicFilter(t.Topic)))
            {
                AckPacket.ReasonCodes = this.GetReasonCodes(ReasonCode.PROTOCOL_ERROR, packet.TopicFilters.Count);
                AckPacket.ReasonString = result.Message = $"UNSUBSCRIBE TopicName is invalid";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }

            var ClientData = client.GetClientData();
            if (ClientData.TopicFilters == null || ClientData.TopicFilters.Count == 0)
            {
                AckPacket.ReasonCodes = this.GetReasonCodes(ReasonCode.NO_SUBSCRIPTION_EXISTED, packet.TopicFilters.Count); AckPacket.ReasonString = result.Message = $"UNSUBSCRIBE TopicName is not exist";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            var reasonCodes = new List<ReasonCode>();
            var index = 0;
            do
            {
                var t = packet.TopicFilters[index++];
                if (MqttHelper.IsValidTopicFilter(t.Topic))
                {
                    var anyIndex = ClientData.TopicFilters.FindIndex(a => a.Topic == t.Topic);
                    if (anyIndex > -1)
                    {
                        ClientData.TopicFilters.RemoveAt(anyIndex);
                        reasonCodes.Add(ReasonCode.SUCCESS);
                    }
                    else
                    {
                        reasonCodes.Add(ReasonCode.NO_SUBSCRIPTION_EXISTED); continue;
                    }
                }
                else
                {
                    reasonCodes.Add(ReasonCode.TOPIC_NAME_INVALID);
                }
            } while (index < packet.TopicFilters.Count);

            AckPacket.ReasonCodes = reasonCodes;
            AckPacket.PacketIdentifier = packet.PacketIdentifier;
            AckPacket.UserProperties = packet.UserProperties;
            result.ResultType = ResultType.Success;
            result.MqttPacket = AckPacket;
            if (!await this.SendAsync(AckPacket, client).ConfigureAwait(false)) result.ResultType = ResultType.Error;
            return result;
        }
        #endregion

        #region 发送ping应答
        /// <summary>
        /// 发送ping应答
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">ping请求包</param>
        /// <returns></returns>
        public async Task<ResultPacket> PingRespAsync(ISocketClient client, PingReqPacket packet)
        {
            var result = new ResultPacket(packet, ResultType.Success, "");
            var AckPacket = new PingRespPacket(packet.ProtocolVersion);
            result.MqttPacket = AckPacket;
            if (!await this.SendAsync(AckPacket, client).ConfigureAwait(false)) result.ResultType = ResultType.Error;
            return result;
        }
        #endregion

        #region 处理Disconnect请求
        /// <summary>
        /// 处理Disconnect请求
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">断开包</param>
        /// <returns></returns>
        private async Task<ResultPacket> ProcessDisconnectAsync(ISocketClient client, DisconnectPacket packet)
        {
            var result = new ResultPacket(packet, ResultType.Error, "");
            var AckPacket = new DisconnectPacket();

            if (packet.FixedFlags != 0x00)
            {
                AckPacket.ReasonCode = ReasonCode.MALFORMED_PACKET;
                AckPacket.ReasonString = result.Message = $"DISCONNECT FixedFlags is not 0x00";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }

            result.ResultType = ResultType.Success;
            result.MqttPacket = AckPacket;
            return result;
        }
        #endregion

        #region 发送断开包
        /// <summary>
        /// 发送断开包
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">断开包</param>
        /// <returns></returns>
        public async Task<ResultPacket> DisconnectAsync(ISocketClient client, DisconnectPacket packet)
        {
            var result = new ResultPacket(packet, ResultType.Success, "");
            if (!await this.SendAsync(packet, client).ConfigureAwait(false)) result.ResultType = ResultType.Error;
            client.Stop();
            return result;
        }
        #endregion

        #region 处理Auth请求
        /// <summary>
        /// 处理Auth请求
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">断开包</param>
        /// <returns></returns>
        private async Task<ResultPacket> ProcessAuthPacketAsync(ISocketClient client, AuthPacket packet)
        {
            var IsConnected = await this.IsConnectedAsync(client).ConfigureAwait(false);
            if (IsConnected.ResultType != ResultType.Success) return IsConnected;

            var result = new ResultPacket(packet, ResultType.Error, "");
            var AckPacket = new AuthPacket();

            if (packet.FixedFlags != 0x00)
            {
                AckPacket.ReasonCode = ReasonCode.MALFORMED_PACKET;
                AckPacket.ReasonString = result.Message = $"AUTH FixedFlags is not 0x00";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }

            result.ResultType = ResultType.Success;
            result.MqttPacket = AckPacket;
            return result;
        }
        #endregion

        #region 判断是否有连接请求
        /// <summary>
        /// 判断是否有连接请求
        /// </summary>
        /// <param name="client">客户端</param>
        /// <returns></returns>
        public async Task<ResultPacket> IsConnectedAsync(ISocketClient client)
        {
            var result = new ResultPacket(ResultType.Error, "");
            var ClientData = client.GetClientData();
            if (ClientData == null || ClientData.ConnectPacket == null)
            {
                var DisconnPacket = new DisconnectPacket()
                {
                    ReasonCode = ReasonCode.DISCONNECT_WITH_WILL_MESSAGE,
                    ReasonString = "Please send the connection package before proceeding"
                };
                result.MqttPacket = DisconnPacket;
                await DisconnectAsync(client, DisconnPacket).ConfigureAwait(false);
                OnMessage?.Invoke(client, new ResultPacket(DisconnPacket, $"Sending {DisconnPacket.PacketType} to {client.EndPoint}"));
                return result;
            }
            result.ResultType = ResultType.Success;
            return result;
        }
        #endregion

        #region 发送 PUBLISH 应答
        /// <summary>
        /// 发送 PUBLISH 应答
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">请求包</param>
        /// <returns></returns>
        public async Task<ResultPacket> PubAckAsync(ISocketClient client, PublishPacket packet)
        {
            var IsConnected = await this.IsConnectedAsync(client).ConfigureAwait(false);
            if (IsConnected.ResultType != ResultType.Success) return IsConnected;

            var result = new ResultPacket(packet, ResultType.Error, "");
            var AckPacket = new MqttPubAckPacket();

            if (this.ServerOptions.MaximumPacketSize > 0 && packet.PacketSize > this.ServerOptions.MaximumPacketSize)
            {
                AckPacket.ReasonCode = ReasonCode.PACKET_TOO_LARGE;
                AckPacket.ReasonString = result.Message = $"PUBLISH packet too large";
                AckPacket.PacketType = packet.QualityOfServiceLevel == QualityOfServiceLevel.AtLeastOnce ? PacketType.PUBACK : PacketType.PUBREC;
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }

            if (packet.QualityOfServiceLevel == QualityOfServiceLevel.Reserved)
            {
                var DisconnPacket = new DisconnectPacket()
                {
                    ReasonCode = ReasonCode.MALFORMED_PACKET,
                    ProtocolVersion = packet.ProtocolVersion,
                    ReasonString = "PUBLISH QoS is 0x03",
                    UserProperties = packet.UserProperties
                };
                result.MqttPacket = DisconnPacket;
                await DisconnectAsync(client, DisconnPacket).ConfigureAwait(false);
                OnMessage?.Invoke(client, new ResultPacket(DisconnPacket, DisconnPacket.ReasonString));
                return result;
            }

            if (packet.QualityOfServiceLevel > QualityOfServiceLevel.Reserved)
            {
                var DisconnPacket = new DisconnectPacket()
                {
                    ReasonCode = ReasonCode.QOS_NOT_SUPPORTED,
                    ProtocolVersion = packet.ProtocolVersion,
                    ReasonString = "PUBLISH QoS not supported",
                    UserProperties = packet.UserProperties
                };
                result.MqttPacket = DisconnPacket;
                await DisconnectAsync(client, DisconnPacket).ConfigureAwait(false);
                OnMessage?.Invoke(client, new ResultPacket(DisconnPacket, DisconnPacket.ReasonString));
                return result;
            }

            var ClientData = client.GetClientData();

            if (!ClientData.ConnectPacket.WillRetain && packet.Retain)
            {
                var DisconnPacket = new DisconnectPacket()
                {
                    ReasonCode = ReasonCode.RETAIN_NOT_SUPPORTED,
                    ProtocolVersion = packet.ProtocolVersion,
                    ReasonString = "PUBLISH retain not supported",
                    UserProperties = packet.UserProperties
                };
                result.MqttPacket = DisconnPacket;
                await DisconnectAsync(client, DisconnPacket).ConfigureAwait(false);
                OnMessage?.Invoke(client, new ResultPacket(DisconnPacket, DisconnPacket.ReasonString));
                return result;
            }

            if (!MqttHelper.IsValidTopicName(packet.Topic))
            {
                AckPacket.ReasonCode = ReasonCode.PROTOCOL_ERROR;
                AckPacket.ReasonString = result.Message = $"PUBLISH topic name is invalid";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }

            if (packet.QualityOfServiceLevel > QualityOfServiceLevel.AtMostOnce && packet.PacketIdentifier <= 0)
            {
                AckPacket.ReasonCode = ReasonCode.PACKET_IDENTIFIER_NOT_FOUND;
                AckPacket.ReasonString = result.Message = $"PUBLISH packet identifier is null or empty";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }

            if (packet.Payload == null || packet.Payload.Length == 0)
            {
                AckPacket.ReasonCode = ReasonCode.PAYLOAD_FORMAT_INVALID;
                AckPacket.ReasonString = result.Message = $"PUBLISH packet payload is null or empty";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }
            if (packet.QualityOfServiceLevel == QualityOfServiceLevel.AtLeastOnce)
                AckPacket = (PubAckPacket)AckPacket;
            else if (packet.QualityOfServiceLevel == QualityOfServiceLevel.ExactlyOnce)
                AckPacket = (PubRecPacket)AckPacket;

            AckPacket.ReasonCode = ReasonCode.SUCCESS;
            AckPacket.PacketIdentifier = packet.PacketIdentifier;
            AckPacket.UserProperties = packet.UserProperties;
            result.ResultType = ResultType.Success;
            result.MqttPacket = AckPacket;
            if ((packet.QualityOfServiceLevel == QualityOfServiceLevel.AtLeastOnce || packet.QualityOfServiceLevel == QualityOfServiceLevel.ExactlyOnce) && !await this.SendAsync(AckPacket, client).ConfigureAwait(false)) result.ResultType = ResultType.Error;
            this.PublishAsync(client, packet).ConfigureAwait(false).GetAwaiter();
            this.SavePublishAsync(packet).ConfigureAwait(false).GetAwaiter();
            return result;
        }
        #endregion

        #region 发送 PUBREL 应答
        /// <summary>
        /// 发送 PUBREL 应答
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">请求包</param>
        /// <returns></returns>
        public async Task<ResultPacket> PubCompAsync(ISocketClient client, PubRelPacket packet)
        {
            var IsConnected = await this.IsConnectedAsync(client).ConfigureAwait(false);
            if (IsConnected.ResultType != ResultType.Success) return IsConnected;

            var result = new ResultPacket(packet, ResultType.Error, "");
            var AckPacket = new PubCompPacket();

            if (packet.FixedFlags != 0x02)
            {
                AckPacket.ReasonCode = ReasonCode.MALFORMED_PACKET;
                AckPacket.ReasonString = result.Message = $"UNSUBSCRIBE FixedFlags is not 0x02";
                await SendAsync(AckPacket, client).ConfigureAwait(false);
                return result;
            }

            AckPacket.ReasonCode = ReasonCode.SUCCESS;
            AckPacket.PacketIdentifier = packet.PacketIdentifier;
            AckPacket.UserProperties = packet.UserProperties;
            result.ResultType = ResultType.Success;
            result.MqttPacket = AckPacket;
            if (!await this.SendAsync(AckPacket, client).ConfigureAwait(false)) result.ResultType = ResultType.Error;
            return result;
        }
        #endregion

        #region 分发数据
        /// <summary>
        /// 分发数据
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">数据包</param>
        /// <returns></returns>
        public async Task PublishAsync(ISocketClient client, PublishPacket packet)
        {
            var cdata = client.GetClientData();
            if (cdata == null || cdata.ConnectPacket == null) await Task.CompletedTask;
            var IsSendShare = false;
            this.Server.Clients.Each(async c =>
            {
                var data = c.GetClientData();
                if (data == null || data.ConnectPacket == null || data.TopicFilters == null || data.TopicFilters.Count == 0) return;
                if (data.ConnectPacket.ClientId == cdata.ConnectPacket.ClientId) return;
                var topicFilter = data.TopicFilters.Find(t => MqttHelper.IsContainsTopic(t.Topic, packet.Topic, this.ServerOptions.WildcardSubscriptionAvailable));
                if (topicFilter == null) return;
                else
                {
                    if (MqttHelper.IsSharedTopicFilter(topicFilter.Topic))
                    {
                        if (IsSendShare)
                            return;
                        else
                            IsSendShare = true;
                    }
                    var result = new ResultPacket(packet, ResultType.Error, "");
                    packet.ProtocolVersion = data.ConnectPacket.ProtocolVersion;
                    var flags = await SendAsync(packet, c).ConfigureAwait(false);
                    if (flags)
                    {
                        result.ResultType = ResultType.Success;
                        result.Message = $"Sending PUBLISH to {data.ConnectPacket.ClientId} ({packet})";
                    }
                    else
                        result.Message = $"Sending PUBLISH to {cdata.ConnectPacket.ClientId} failed.";
                    OnMessage?.Invoke(c, result);
                }
            });
        }
        #endregion

        #region 保存分发数据
        /// <summary>
        /// 保存分发数据
        /// </summary>
        /// <param name="packet">分发数据</param>
        /// <returns></returns>
        public async Task<bool> SavePublishAsync(PublishPacket packet)
        {
            if (!this.ServerOptions.RetainAvailable || !packet.Retain) return await Task.FromResult(false);
            if (this.MqttServerTopicMessages == null)
                this.MqttServerTopicMessages = new ConcurrentDictionary<string, IMqttServerTopicMessage>();
            var topicMessage = MqttServerTopicMessage.Create(packet, DateTime.Now.ToTimeStamp() + this.ServerOptions.TopicFilterExpireInterval);
            this.MqttServerTopicMessages.AddOrUpdate(packet.Topic, topicMessage, (key, old) => topicMessage);
            return await Task.FromResult(true);
        }
        #endregion

        #region 发送保存分发数据
        /// <summary>
        /// 发送保存分发数据
        /// </summary>
        /// <param name="client">MQTT客户端</param>
        /// <param name="topicFilter">主题过滤器</param>
        /// <returns></returns>
        public async Task<bool> SendPublishAsync(ISocketClient client, TopicFilter topicFilter)
        {
            if (!this.ServerOptions.RetainAvailable || this.MqttServerTopicMessages == null || this.MqttServerTopicMessages.Count == 0 || !MqttHelper.IsValidTopicFilter(topicFilter.Topic)) return await Task.FromResult(false);

            this.MqttServerTopicMessages.Each(m =>
            {
                var topic = m.Key;
                var msg = m.Value;
                var packet = msg.PublishPacket;
                if (MqttHelper.IsContainsTopic(topicFilter.Topic, topic, this.ServerOptions.WildcardSubscriptionAvailable) && topicFilter.QualityOfServiceLevel <= packet.QualityOfServiceLevel)
                {
                    packet.ProtocolVersion = client.GetClientData().ConnectPacket.ProtocolVersion;
                    this.SendAsync(packet, client).ConfigureAwait(false).GetAwaiter();
                    if (packet.QualityOfServiceLevel == QualityOfServiceLevel.AtMostOnce || packet.QualityOfServiceLevel == QualityOfServiceLevel.ExactlyOnce || msg.DistributeCount > this.ServerOptions.MaximumDistribute)
                        this.MqttServerTopicMessages.TryRemove(topic, out _);
                    else
                        msg.AddDistributeCount();
                }
                else
                {
                    if (msg.ExpiredTime < DateTime.Now.ToTimeStamp() || msg.DistributeCount > this.ServerOptions.MaximumDistribute)
                        this.MqttServerTopicMessages.TryRemove(topic, out _);
                }
            });
            return await Task.FromResult(true);
        }
        #endregion

        #region 发送数据
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="packet">消息包</param>
        /// <param name="client">客户端</param>
        /// <returns></returns>
        public async Task<bool> SendAsync(MqttPacket packet, ISocketClient client = null)
        {
            if (!this.Server.Active) this.Start();
            if (packet == null || this.Server.Clients == null || this.Server.Clients.Count == 0) return false;
            if (client != null && client.Active && client.Connected)
            {
                return (await client.SendAsync(packet.ToArray(), MessageType.Binary).ConfigureAwait(false)) > 0;
            }
            if (this.Server.Clients.Count == 0) return false;
            this.Server.Clients.Each(async c =>
            {
                await c.SendAsync(packet.ToArray(), MessageType.Binary).ConfigureAwait(false);
            });
            return true;
        }
        #endregion

        #region 凭证
        /// <summary>
        /// 添加凭证
        /// </summary>
        /// <param name="credential">用户凭证</param>
        public void AddCredential(IMqttServerCredential credential)
        {
            if (this.MqttServerCredentials == null) this.MqttServerCredentials = new ConcurrentDictionary<string, IMqttServerCredential>();
            this.MqttServerCredentials.AddOrUpdate(credential.UserName, credential, (k, v) => credential);
        }
        /// <summary>
        /// 添加凭证
        /// </summary>
        /// <param name="userName">帐号</param>
        /// <param name="password">密码</param>
        public void AddCredential(string userName, string password) => this.AddCredential(userName, password, "");
        /// <summary>
        /// 添加凭证
        /// </summary>
        /// <param name="userName">帐号</param>
        /// <param name="password">密码</param>
        /// <param name="allowClientIp">允许IP</param>
        public void AddCredential(string userName, string password, string allowClientIp) => this.AddCredential(userName, password, allowClientIp.IsNullOrEmpty() ? null : new List<string> { allowClientIp });
        /// <summary>
        /// 添加凭证
        /// </summary>
        /// <param name="userName">帐号</param>
        /// <param name="password">密码</param>
        /// <param name="allowClientIp">允许IP</param>
        public void AddCredential(string userName, string password, IList<string> allowClientIp) => this.AddCredential(MqttServerCredential.Create(userName, password, allowClientIp));
        /// <summary>
        /// 添加凭证
        /// </summary>
        /// <param name="userName">帐号</param>
        /// <param name="password">密码</param>
        /// <param name="allowClientIp">允许IP</param>
        public void AddCredential(string userName, string password, params string[] allClientIp) => this.AddCredential(userName, password, allClientIp);
        /// <summary>
        /// 设置当前帐号允许IP
        /// </summary>
        /// <param name="userName">帐号</param>
        /// <param name="allowClientIp">允许IP</param>
        public void AddCredentialAllowClientIp(string userName, string allowClientIp)
        {
            if (this.MqttServerCredentials == null) this.MqttServerCredentials = new ConcurrentDictionary<string, IMqttServerCredential>();
            if (this.MqttServerCredentials.TryGetValue(userName, out var credential))
            {
                if (credential.AllowClientIp == null) credential.AllowClientIp = new List<string>();
                if (!credential.AllowClientIp.Contains(allowClientIp))
                    credential.AllowClientIp.Add(allowClientIp);
            }
        }
        /// <summary>
        /// 移除凭证
        /// </summary>
        /// <param name="userName">帐号</param>
        /// <returns></returns>
        public bool RemoveCredential(string userName)
        {
            if (this.MqttServerCredentials == null) return false;
            return this.MqttServerCredentials.TryRemove(userName, out var _);
        }
        #endregion

        #region 服务器转移到其它服务器
        /// <summary>
        /// 服务器转移到其它服务器
        /// </summary>
        /// <param name="client">MQTT客户端</param>
        /// <returns></returns>
        public async Task<ResultPacket> ServerMovedAsync(ISocketClient client)
        {
            var result = new ResultPacket(ResultType.Error, $"Server Moved {this.ServerOptions.ServerReference}.");
            var DisconnPacket = new DisconnectPacket()
            {
                ReasonCode = ReasonCode.SERVER_MOVED,
                ReasonString = result.Message
            };
            result.MqttPacket = DisconnPacket;
            await DisconnectAsync(client, DisconnPacket).ConfigureAwait(false);
            return result;
        }
        #endregion

        #region 释放
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">标识</param>
        protected override void Dispose(bool disposing)
        {
            this.Stop();
            if (this.MqttServerCredentials != null)
            {
                this.MqttServerCredentials.Clear();
                this.MqttServerCredentials = null;
            }
            if (this.MqttServerTopicMessages != null)
            {
                this.MqttServerTopicMessages.Clear();
                this.MqttServerTopicMessages = null;
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~MqttServer()
        {
            this.Dispose(false);
        }
        #endregion

        #endregion
    }
}