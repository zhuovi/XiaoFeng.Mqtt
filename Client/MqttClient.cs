﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Mqtt.Internal;
using XiaoFeng.Mqtt.Packets;
using XiaoFeng.Net;
using XiaoFeng.Threading;
/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-27 13:57:49                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Client
{
    /// <summary>
    /// MQTT客户端
    /// </summary>
    public class MqttClient : Disposable, IMqttClient
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MqttClient()
        {

        }
        /// <summary>
        /// 设置服务端终节点
        /// </summary>
        /// <param name="url">网址</param>
        public MqttClient(string url):this(new NetUri(url))
        {

        }
        /// <summary>
        /// 设置服务端终节点
        /// </summary>
        /// <param name="netUri">网络地址</param>
        public MqttClient(NetUri netUri)
        {
            this.NetUri= netUri;
        }
        /// <summary>
        /// 设置服务器地址和端口
        /// </summary>
        /// <param name="host">服务器地址</param>
        /// <param name="port">服务器端口</param>
        public MqttClient(string host, int port)
        {
            this.NetUri= new NetUri(NetType.Tcp, host, port);
        }
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="options">配置</param>
        public MqttClient(MqttClientOptions options)
        {
            ClientOptions = options;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 网络地址
        /// </summary>
        private NetUri NetUri { get; set; }
        /// <summary>
        /// 客户端配置
        /// </summary>
        public MqttClientOptions ClientOptions { get; set; }
        /// <summary>
        /// 连接状态
        /// </summary>
        private bool _Connected = false;
        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool Connected => this._Connected;
        /// <summary>
        /// 客户端
        /// </summary>
        private ISocketClient Client { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        private bool _Active = false;
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Active => this._Active;
        /// <summary>
        /// ping作业
        /// </summary>
        private IJob PingJob { get; set; }
        /// <summary>
        /// 我的订阅
        /// </summary>
        public ConcurrentDictionary<string, TopicFilter> TopicFilters { get; set; } = new ConcurrentDictionary<string, TopicFilter>();
        #endregion

        #region 事件
        /// <summary>
        /// 连接事件
        /// </summary>
        public event MqttClientConnectedEventHandler OnConnected;
        /// <summary>
        /// 出错事件
        /// </summary>
        public event MqttClientErrorEventHandler OnError;
        /// <summary>
        /// 消息事件
        /// </summary>
        public event MqttClientMessageEventHandler OnMessage;
        /// <summary>
        /// 断开事件
        /// </summary>
        public event MqttClientDisconnectedEventHandler OnDisconnected;
        /// <summary>
        /// 启动事件
        /// </summary>
        public event MqttClientStartedEventHandler OnStarted;
        /// <summary>
        /// 停止事件
        /// </summary>
        public event MqttClientStopedEventHandler OnStoped;
        /// <summary>
        /// 接收所订阅的消息事件
        /// </summary>
        public event MqttClientPublishMessageEventHandler OnPublishMessage;
        #endregion

        #region 方法

        #region 初始化
        /// <summary>
        /// 获取Socket
        /// </summary>
        /// <returns></returns>
        private ISocketClient GetClient()
        {
            ISocketClient client = (this.NetUri.NetType == NetType.Ws || this.NetUri.NetType == NetType.Wss) ? new WebSocketClient() : new SocketClient();

            if (this.NetUri.NetType == NetType.Wss || this.NetUri.NetType == NetType.Mqtts)
            {
                client.SslProtocols = this.ClientOptions.SslProtocols;
                if (this.ClientOptions.ClientCertificates != null)
                    client.ClientCertificates = this.ClientOptions.ClientCertificates;
            }
            
            client.ConnectTimeout = (int)this.ClientOptions.Timeout.TotalMilliseconds;
            client.ReceiveBufferSize = this.ClientOptions.ReadeBufferSize;
            client.SendBufferSize = this.ClientOptions.WriteBufferSize;
            client.OnClientError += (o, m, e) =>
            {
                OnError?.Invoke(this, e.Message);
            };
            client.OnMessageByte += (o, m, e) =>
            {
                this.ReceiveMessage(m);
            };
            client.OnStart += (o, e) =>
            {
                this._Active = true;
                OnStarted?.Invoke(this);
            };
            client.OnStop += async (o, e) =>
            {
                this._Connected = false;
                this._Active = false;
                if (PingJob != null)
                {
                    this.PingJob.Stop();
                    this.PingJob = null;
                }
                OnStoped?.Invoke(this);
                if (this.ClientOptions.IsAutoConnect)
                {
                    await Task.Delay(this.ClientOptions.ReConnectPeriod * 1000);
                    this.ConnectAsync().ConfigureAwait(false).GetAwaiter();
                }
            };
            return client;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private async Task<bool> InitAsync()
        {
            if (this.NetUri == null) throw new MqttException("Please configure the server endpoint.");
            if (this.Active) return await Task.FromResult(false);
            if (this.ClientOptions == null)
            {
                this.ClientOptions = new MqttClientOptions();
                if (this.NetUri.UserName.IsNotNullOrEmpty())
                    this.ClientOptions.UserName = this.NetUri.UserName;
                if(this.NetUri.Password.IsNotNullOrEmpty())
                    this.ClientOptions.UserName=this.NetUri.Password;
                if (this.NetUri.NetType.IsDefined<SslAttribute>())
                    this.ClientOptions.UseTls = true;
                var paramer = this.NetUri.Parameters;
                if (paramer != null && paramer.Count > 0)
                {
                    if (paramer.Contains("connecttimeout"))
                        this.ClientOptions.Timeout = TimeSpan.FromSeconds(paramer["connecttimeout"].ToCast(10));
                    if (paramer.Contains("keepalive"))
                        this.ClientOptions.KeepAlive = paramer["keepalive"].ToCast<ushort>(10);
                    if (paramer.Contains("maxpacketsize"))
                        this.ClientOptions.MaximumPacketSize = paramer["maxpacketsize"].ToCast(0U);
                    if (paramer.Contains("maxqos"))
                        this.ClientOptions.WillQoS = paramer["maxqos"].ToCast<QualityOfServiceLevel>();
                    if (paramer.Contains("readtimeout"))
                        this.ClientOptions.ReadTimeout = paramer["readtimeout"].ToCast(10);
                    if (paramer.Contains("writetimeout"))
                        this.ClientOptions.WriteTimeout = paramer["writetimeout"].ToCast(10);
                    if (paramer.Contains("writebuffersize"))
                        this.ClientOptions.WriteBufferSize = paramer["writebuffersize"].ToCast(8192);
                    if (paramer.Contains("readbuffersize"))
                        this.ClientOptions.ReadeBufferSize = paramer["readbuffersize"].ToCast(8192);
                }
            }
            if (this.Client == null)
            {
                this.Client = this.GetClient();
            }
            SocketError status;
            if (Client is IWebSocketClient ws)
            {
                var netUri = this.NetUri;
                netUri.NetType = netUri.NetType.IsDefined<SslAttribute>() ? NetType.Wss : NetType.Ws;
                status = ws.Connect(new Uri(netUri.ToString()));
            }
            else if (Client is ISocketClient socket)
                status = socket.Connect(this.NetUri.Host, this.NetUri.Port);
            else
                throw new MqttException("Please configure the server endpoint.");

            //var status = this.Client.Connect(this.EndPoint);
            if (status == SocketError.Success)
            {
                this._Active = true;
                this.Client.StartEventHandler();
                return await Task.FromResult(true);
            }
            else
            {
                this.OnError?.Invoke(this, "Connect failed.");
                return await Task.FromResult(false);
            }
        }
        #endregion

        #region 接收数据
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="bytes">字节流</param>
        private void ReceiveMessage(byte[] bytes)
        {
            var packetType = (PacketType)(bytes[0] >> 4);
            ResultPacket result = null;
            DisconnectPacket disconnPacket;
            switch (packetType)
            {
                case PacketType.PUBLISH:
                    var publishPacket = new PublishPacket(bytes, this.ClientOptions.ProtocolVersion);
                    OnMessage?.Invoke(result = new ResultPacket(ResultType.Success, $"Received {publishPacket.PacketType} from server ({publishPacket})."));
                    if (publishPacket.QualityOfServiceLevel == QualityOfServiceLevel.Reserved)
                    {
                        disconnPacket = new DisconnectPacket
                        {
                            ReasonCode = ReasonCode.MALFORMED_PACKET,
                            ReasonString = "PUBLISH QoS is 0x03"
                        };
                        OnError?.Invoke(this, disconnPacket.ReasonString);
                        DisconnectAsync(disconnPacket).ConfigureAwait(false).GetAwaiter();
                        return;
                    }
                    if (publishPacket.QualityOfServiceLevel > QualityOfServiceLevel.Reserved)
                    {
                        disconnPacket = new DisconnectPacket
                        {
                            ReasonCode = ReasonCode.QOS_NOT_SUPPORTED,
                            ReasonString = "PUBLISH QoS not supported"
                        };
                        OnError?.Invoke(this, disconnPacket.ReasonString);
                        DisconnectAsync(disconnPacket).ConfigureAwait(false).GetAwaiter();
                        return;
                    }
                    OnPublishMessage?.Invoke(publishPacket);
                    return;
                case PacketType.DISCONNECT:
                    disconnPacket = new DisconnectPacket(bytes, this.ClientOptions.ProtocolVersion);
                    result = new ResultPacket(ResultType.Success, $"Received {disconnPacket.PacketType} from server ({disconnPacket}).");
                    if (disconnPacket.ServerReference.IsNotNullOrEmpty())
                    {
                        ServerMovedAsync(disconnPacket.ServerReference).ConfigureAwait(false).GetAwaiter();
                    }
                    break;
                case PacketType.PINGRESP:
                    var pingRespPacket = new PingRespPacket(bytes, this.ClientOptions.ProtocolVersion);
                    result = new ResultPacket(ResultType.Success, $"Received {pingRespPacket.PacketType} from server ({pingRespPacket}).");
                    break;
                case PacketType.PUBACK:
                    var pubAckPacket = new PubAckPacket(bytes, this.ClientOptions.ProtocolVersion);
                    result = new ResultPacket(ResultType.Success, $"Received {pubAckPacket.PacketType} from server ({pubAckPacket}).");
                    break;
                case PacketType.PUBREC:
                    var recPacket = new PubRecPacket(bytes, this.ClientOptions.ProtocolVersion);
                    OnMessage?.Invoke(new ResultPacket(ResultType.Success, $"Received {recPacket.PacketType} from server ({recPacket})."));
                    if (recPacket.FixedFlags != 0x02)
                    {
                        disconnPacket = new DisconnectPacket
                        {
                            ReasonCode = ReasonCode.MALFORMED_PACKET,
                            ReasonString = "PUBREC malformed packet"
                        };
                        OnError?.Invoke(this, disconnPacket.ReasonString);
                        DisconnectAsync(disconnPacket).ConfigureAwait(false).GetAwaiter();
                        return;
                    }
                    else
                    {
                        var relPacket = new PubRelPacket(this.ClientOptions.ProtocolVersion)
                        {
                            PacketIdentifier = recPacket.PacketIdentifier,
                            UserProperties = recPacket.UserProperties
                        };
                        SendAsync(relPacket).ConfigureAwait(false);
                        return;
                    }
                case PacketType.PUBCOMP:
                    var pubCompPacket = new PubCompPacket(bytes, this.ClientOptions.ProtocolVersion);
                    result = new ResultPacket(ResultType.Success, $"Received {pubCompPacket.PacketType} from server ({pubCompPacket}).");
                    break;
                case PacketType.SUBACK:
                    var subAckPacket = new SubAckPacket(bytes, this.ClientOptions.ProtocolVersion);
                    result = new ResultPacket(ResultType.Success, $"Received {subAckPacket.PacketType} from server ({subAckPacket}).");
                    break;
                case PacketType.UNSUBACK:
                    var unsubAckPacket = new UnsubAckPacket(bytes, this.ClientOptions.ProtocolVersion);
                    result = new ResultPacket(ResultType.Success, $"Received {unsubAckPacket.PacketType} from server ({unsubAckPacket}).");
                    break;
                default:
                    break;
            }
            if (result != null)
                OnMessage?.Invoke(result);
        }
        #endregion

        #region 连接
        /// <summary>
        /// 连接服务端
        /// </summary>
        /// <returns></returns>
        public async Task<ConnectActPacket> ConnectAsync()
        {
            if (!this.Active)
            {
                var init = await this.InitAsync().ConfigureAwait(false);
                if (!init) return null;
            }
            var connPacket = this.ClientOptions as ConnectPacket;
            OnMessage?.Invoke(new ResultPacket(connPacket, ResultType.Success, $"Sending {connPacket.PacketType} to server ({connPacket})."));
            await this.Client.SendAsync(connPacket.ToArray(), MessageType.Binary).ConfigureAwait(false);
            var bytes = await this.Client.ReceviceMessageAsync().ConfigureAwait(false);
            if(this.Client is IWebSocketClient)
            {
                var packet = new WebSocketPacket(this.Client);
                bytes = packet.UnPacket(bytes);
            }
            var connAct = new ConnectActPacket(bytes);
            OnMessage?.Invoke(new ResultPacket(connAct,resultType: ResultType.Success, $"Received {connAct.PacketType} from server ({connAct})."));
            if (connAct.ReturnCode != ConnectReturnCode.ACCEPTED)
            {
                OnError?.Invoke(this, $"Connect failed: [{connAct.ReasonCode}] [{connAct.ReasonString}]");
                if (connAct.ServerReference.IsNotNullOrEmpty())
                {
                    await ServerMovedAsync(connAct.ServerReference).ConfigureAwait(false);
                    return await Task.FromResult(connAct);
                }
            }
            else
            {
                if (this.ClientOptions.ClientId.IsNullOrEmpty())
                    this.ClientOptions.ClientId = connAct.AssignedClientIdentifier;

                if (connAct.ServerKeepAlive > 0)
                    this.ClientOptions.KeepAlive = connAct.ServerKeepAlive;
                if(connAct.MaximumPacketSize > 0)
                    this.ClientOptions.MaximumPacketSize = connAct.MaximumPacketSize;
                if(connAct.MaximumQoS > 0)
                    this.ClientOptions.WillQoS = connAct.MaximumQoS;
                if(connAct.ReceiveMaximum > 0)
                    this.ClientOptions.ReceiveMaximum = connAct.ReceiveMaximum;
                if(connAct.RetainAvailable)
                    this.ClientOptions.WillRetain = connAct.RetainAvailable;
                if(connAct.SessionExpiryInterval > 0)
                    this.ClientOptions.SessionExpiryInterval = connAct.SessionExpiryInterval;
            }
            this._Connected = true;
            this.Client.ReceviceDataAsync().ConfigureAwait(false).GetAwaiter();
            this.PingWorker();
            this.ReSubscributeAsync().ConfigureAwait(false).GetAwaiter();
            return await Task.FromResult(connAct);
        }
        #endregion

        #region 断开连接
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DisconnectAsync()
        {
            return await DisconnectAsync(new DisconnectPacket()
            {
                ReasonCode = ReasonCode.SUCCESS
            });
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="packet">断开包</param>
        /// <returns></returns>
        public async Task<bool> DisconnectAsync(DisconnectPacket packet)
        {
            var result = await this.SendAsync(packet).ConfigureAwait(false);
            this.Stop();
            return result;
        }
        #endregion

        #region Ping作业
        /// <summary>
        /// Ping作业
        /// </summary>
        private void PingWorker()
        {
            this.PingJob = new Job().SetName("EELF_MQTT_Worker").Interval((int)this.ClientOptions.KeepAlive * 1000, async job =>
            {
                await this.PingAsync().ConfigureAwait(false);
            }).SetStartTime(DateTime.Now.AddMilliseconds(this.ClientOptions.KeepAlive));
            this.PingJob.Start();
        }
        #endregion

        #region 订阅
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="qos">服务质量等级</param>
        /// <returns></returns>
        public async Task<SubscribeResult> SubscributeAsync(string topic, QualityOfServiceLevel qos = QualityOfServiceLevel.AtLeastOnce)
        {
            return await SubscributeAsync(new TopicFilter
            {
                QualityOfServiceLevel = qos,
                Topic = topic
            }).ConfigureAwait(false);
        }
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topicFilter">订阅过滤器</param>
        /// <returns></returns>
        public async Task<SubscribeResult> SubscributeAsync(TopicFilter topicFilter)
        {
            return (await SubscributeAsync(new List<TopicFilter> { topicFilter }).ConfigureAwait(false)).FirstOrDefault();
        }
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topics">主题</param>
        /// <param name="qos">服务质量等级</param>
        /// <returns></returns>
        public async Task<IList<SubscribeResult>> SubscributeAsync(ICollection<KeyValuePair<string, QualityOfServiceLevel>> topics)
        {
            var list = from t in topics select new TopicFilter(t.Key, t.Value);
            return await SubscributeAsync(list).ConfigureAwait(false);
        }
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topicFilters">订阅过滤器</param>
        /// <returns></returns>
        public async Task<IList<SubscribeResult>> SubscributeAsync(IEnumerable<TopicFilter> topicFilters)
        {
            if (topicFilters == null || topicFilters.Count() == 0) return new List<SubscribeResult>();
            var subPacket = new SubscribePacket
            {
                PacketIdentifier = (ushort)RandomHelper.GetRandomInt(1000, 65535),
                SubscriptionIdentifier = (uint)RandomHelper.GetRandomInt(1, 268435450)
            };
            if (this.TopicFilters == null)
            {
                this.TopicFilters = new ConcurrentDictionary<string, TopicFilter>();
            }
            var filter = new List<SubscribeResult>();
            if (this.TopicFilters.Count > 0)
            {
                var list = new List<TopicFilter>();
                topicFilters.Each(t =>
                {
                    var result = new SubscribeResult(t);
                    if (!MqttHelper.IsValidTopicFilter(result.Topic))
                    {
                        result.Status = false;
                        result.Message = $"The subscribed topic '{t.Topic}' is invalid.";
                        filter.Add(result);
                        return;
                    }
                    var b = this.TopicFilters.Keys.Where(a => a == result.Topic || result.Topic.StartsWith(a.TrimEnd(new char[] { '#', '/' })));
                    if (b.Count() == 0)
                    {
                        list.Add(result.TopicFilter);
                        result.Status = true;
                        this.TopicFilters.TryAdd(result.Topic, result.TopicFilter);
                    }
                    else
                    {
                        result.Message = $"The current topic '{t.Topic}' is already subscribed.";
                        result.Status = false;
                    }
                });
                if (list.Count == 0) return filter;
                topicFilters = list;
            }
            else
            {
                topicFilters.Each(t =>
                {
                    var result = new SubscribeResult(t);
                    t.Topic = result.Topic;
                    if (!MqttHelper.IsValidTopicFilter(result.Topic))
                    {
                        result.Status = false;
                        result.Message = $"The subscribed topic {t.Topic} is invalid.";
                        filter.Add(result);
                        return;
                    }
                    this.TopicFilters.TryAdd(result.Topic, result.TopicFilter);
                    filter.Add(result);
                });
            }
            subPacket.TopicFilters.AddRange(topicFilters);
            var flag = await this.SendAsync(subPacket).ConfigureAwait(false);
            if (!flag)
            {
                filter.Each(f =>
                {
                    f.Status = false;
                    f.Message += $" Subscription failed.";
                });
            }
            return filter;
        }
        #endregion

        #region 取消订阅
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="qos">服务质量等级</param>
        /// <returns></returns>
        public async Task<UnsubscribeResult> UnsubscributeAsync(string topic, QualityOfServiceLevel qos = QualityOfServiceLevel.AtLeastOnce)
        {
            return await UnsubscributeAsync(new TopicFilter
            {
                QualityOfServiceLevel = qos,
                Topic = topic
            }).ConfigureAwait(false);
        }
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topicFilter">订阅过滤器</param>
        /// <returns></returns>
        public async Task<UnsubscribeResult> UnsubscributeAsync(TopicFilter topicFilter)
        {
            return (await UnsubscributeAsync(new List<TopicFilter> { topicFilter }).ConfigureAwait(false)).FirstOrDefault();
        }
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topics">主题</param>
        /// <param name="qos">服务质量等级</param>
        /// <returns></returns>
        public async Task<IList<UnsubscribeResult>> UnsubscributeAsync(ICollection<KeyValuePair<string, QualityOfServiceLevel>> topics)
        {
            var list = from t in topics select new TopicFilter(t.Key, t.Value);
            return await UnsubscributeAsync(list).ConfigureAwait(false);
        }
        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="topicFilters">订阅主题</param>
        /// <returns></returns>
        public async Task<IList<UnsubscribeResult>> UnsubscributeAsync(IEnumerable<TopicFilter> topicFilters)
        {
            if (topicFilters == null || topicFilters.Count() == 0) return new List<UnsubscribeResult>();
            if (this.TopicFilters == null || this.TopicFilters.Count == 0) return new List<UnsubscribeResult>();
            var filter = new List<UnsubscribeResult>();
            var subPacket = new UnsubscribePacket
            {
                PacketIdentifier = (ushort)RandomHelper.GetRandomInt(1000, 65535)
            };
            if (this.TopicFilters.Count > 0)
            {
                var list = new List<TopicFilter>();
                topicFilters.Each((Action<TopicFilter>)(t =>
                {
                    var result = new UnsubscribeResult(t.Topic);
                    if (!MqttHelper.IsValidTopicFilter(result.Topic))
                    {
                        result.Status = false;
                        result.Message = $"Unsubscribed topic '{t}' is invalid.";
                        filter.Add(result);
                        return;
                    }
                    var b = this.TopicFilters.Keys.Where(a => a == result.Topic || result.Topic.StartsWith(a.TrimEnd(new char[] { '#', '/' })));
                    if (b.Count() > 0)
                    {
                        list.Add(result.Topic);
                        result.Status = true;
                    }
                    else
                    {
                        result.Message = $"Not subscribed to the current topic '{t}' yet .";
                        result.Status = false;
                    }
                }));
                if (list.Count == 0) return filter;
                topicFilters = list;
            }
            subPacket.TopicFilters.AddRange(topicFilters);
            var flag = await this.SendAsync(subPacket).ConfigureAwait(false);
            if (!flag)
            {
                filter.Each(f =>
                {
                    f.Status = false;
                    f.Message += $" Unsubscribe failed.";
                });
            }
            return filter;
        }
        #endregion

        #region 发布
        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="data">数据</param>
        /// <param name="qos">服务质量等级</param>
        /// <returns></returns>
        public async Task<bool> PublishAsync(string topic, object data, QualityOfServiceLevel qos = QualityOfServiceLevel.AtLeastOnce)
        {
            if (data.IsNullOrEmpty()) return false;
            if (qos > this.ClientOptions.WillQoS)
                qos = this.ClientOptions.WillQoS;
            topic = topic.Trim('/');
            if(!MqttHelper.IsValidTopicName(topic))return false;

            var packet = new PublishPacket()
            {
                QualityOfServiceLevel = qos,
                Topic = topic,
                PacketIdentifier = (ushort)RandomHelper.GetRandomInt(1000, 65535)
            };
            var baseType = data.GetType().GetValueType();
            switch (baseType)
            {
                case ValueTypes.String:
                    packet.Payload = data.ToString().GetBytes();
                    break;
                case ValueTypes.Null:
                    return false;
                case ValueTypes.Struct:
                case ValueTypes.Class:
                case ValueTypes.DataTable:
                case ValueTypes.Anonymous:
                case ValueTypes.Array:
                case ValueTypes.ArrayList:
                case ValueTypes.Dictionary:
                    packet.Payload = data.ToJson().GetBytes();
                    break;
                default:
                    return false;
            }
            return await this.PublishAsync(packet);
        }
        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="packet">发布包</param>
        /// <returns></returns>
        public async Task<bool> PublishAsync(PublishPacket packet)
        {
            return await this.SendAsync(packet).ConfigureAwait(false);
        }
        #endregion

        #region Ping
        /// <summary>
        /// 发送Ping
        /// </summary>
        /// <returns></returns>
        public async Task<bool> PingAsync()
        {
            return await this.SendAsync(new PingReqPacket()).ConfigureAwait(false);
        }
        #endregion

        #region 认证
        /// <summary>
        /// 认证
        /// </summary>
        /// <param name="packet">认证包</param>
        /// <returns></returns>
        public async Task<bool> AuthAsync(AuthPacket packet)
        {
            return await this.SendAsync(packet).ConfigureAwait(false);
        }
        #endregion

        #region 发送
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <returns></returns>
        private async Task<bool> SendAsync(MqttPacket packet)
        {
            packet.ProtocolVersion = this.ClientOptions.ProtocolVersion;

            if (!this.Connected) await this.ConnectAsync().ConfigureAwait(false);

            OnMessage?.Invoke(new ResultPacket(packet, ResultType.Success, $"Sending {packet.PacketType} to server ({packet})."));

            return await this.Client.SendAsync(packet.ToArray(), MessageType.Binary).ConfigureAwait(false) > 0;
        }
        #endregion

        #region 停止
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            this.Client?.Stop();
            if (this.PingJob != null)
            {
                this.PingJob.Stop();
                this.PingJob = null;
            }
        }
        #endregion

        #region 注销
        /// <summary>
        /// 注销
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="disposing">标识</param>
        protected override void Dispose(bool disposing)
        {
            this.Stop();
            this.TopicFilters.Clear();
            this.TopicFilters = null;
            base.Dispose(disposing);
            GC.Collect();
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~MqttClient()
        {
            this.Dispose(true);
        }
        #endregion

        #region 断开前订阅的主题重新订阅
        /// <summary>
        /// 断开前订阅的主题重新订阅
        /// </summary>
        /// <returns></returns>
        private async Task<IList<SubscribeResult>> ReSubscributeAsync()
        {
            if (this.TopicFilters == null || this.TopicFilters.Count == 0) return await Task.FromResult(new List<SubscribeResult>());
            var list = new List<TopicFilter>();
            this.TopicFilters.Each(t =>
            {
                list.Add(t.Value);
            });
            return await this.SubscributeAsync(list).ConfigureAwait(false);
        }
        #endregion

        #region 服务器转移到其它服务器
        /// <summary>
        /// 服务器转移到其它服务器
        /// </summary>
        /// <param name="serverReference">服务器信息</param>
        /// <returns></returns>
        private async Task ServerMovedAsync(string serverReference)
        {
            var reader = new StreamReader(serverReference);
            var line = reader.ReadLine();
            while (line.IsNotNullOrEmpty())
            {
                OnMessage?.Invoke(new ResultPacket(ResultType.Success, $"Server reference: {line}"));
                var hostport = line.Trim().Split(':');
                if (line.Length > 1)
                {
                    this.NetUri.Host = hostport[0];
                    this.NetUri.Port = hostport[1].ToCast(1883);
                }
                else
                {
                    this.NetUri.Host = line.Trim();
                    this.NetUri.Port = 1883;
                }
                this.Stop();
                await this.ConnectAsync().ConfigureAwait(false);
                break;
            }
        }
        #endregion

        #endregion
    }
}