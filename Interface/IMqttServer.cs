using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Mqtt.Packets;
using XiaoFeng.Mqtt.Server;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-27 15:04:47                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt
{
    /// <summary>
    /// Mqtt服务端接口
    /// </summary>
    public interface IMqttServer
    {
        #region 属性
        /// <summary>
        /// 配置
        /// </summary>
        MqttServerOptions ServerOptions { get; }
        /// <summary>
        /// MQTT服务端主题消息
        /// </summary>
        ConcurrentDictionary<string, IMqttServerTopicMessage> MqttServerTopicMessages { get; set; }
        #endregion

        #region 事件
        /// <summary>
        /// 启动事件
        /// </summary>
        event MqttServerStaredEventHandler OnStarted;
        /// <summary>
        /// 停止事件
        /// </summary>
        event MqttServerStopedEventHandler OnStoped;
        /// <summary>
        /// 新连接事件
        /// </summary>
        event MqttServerConnectedEventHandler OnConnected;
        /// <summary>
        /// 断开事件
        /// </summary>
        event MqttServerDisconnectedEventHandler OnDisconnected;
        /// <summary>
        /// 服务端出错事件
        /// </summary>
        event MqttServerErrorEventHandler OnError;
        /// <summary>
        /// 客户端出错事件
        /// </summary>
        event MqttServerClientErrorEventHandler OnClientError;
        /// <summary>
        /// 接收消息事件
        /// </summary>
        event MqttServerMessageEventHandler OnMessage;
        #endregion

        #region 方法
        /// <summary>
        /// 启动
        /// </summary>
        void Start();
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="options">配置</param>
        void Start(MqttServerOptions options);
        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
        /// <summary>
        /// 是否存在ClientId
        /// </summary>
        /// <param name="clientId">客户端Id</param>
        /// <returns></returns>
        bool Contains(string clientId);
        /// <summary>
        /// 发送连接应答
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">连接包</param>
        /// <returns></returns>
        Task<ResultPacket> ConnActAsync(ISocketClient client, ConnectPacket packet);
        /// <summary>
        /// 发送订阅应答
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">订阅包</param>
        /// <returns></returns>
        Task<ResultPacket> SubActAsync(ISocketClient client, SubscribePacket packet);
        /// <summary>
        /// 发送取消订阅应答
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">取消订阅包</param>
        /// <returns></returns>
        Task<ResultPacket> UnsubAckAsync(ISocketClient client, UnsubscribePacket packet);
        /// <summary>
        /// 发送ping应答
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">ping请求包</param>
        /// <returns></returns>
        Task<ResultPacket> PingRespAsync(ISocketClient client, PingReqPacket packet);
        /// <summary>
        /// 发送断开包
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">断开包</param>
        /// <returns></returns>
        Task<ResultPacket> DisconnectAsync(ISocketClient client, DisconnectPacket packet);
        /// <summary>
        /// 判断是否有连接请求
        /// </summary>
        /// <param name="client">客户端</param>
        /// <returns></returns>
        Task<ResultPacket> IsConnectedAsync(ISocketClient client);
        /// <summary>
        /// 发送 PUBLISH 应答
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">请求包</param>
        /// <returns></returns>
        Task<ResultPacket> PubAckAsync(ISocketClient client, PublishPacket packet);
        /// <summary>
        /// 发送 PUBREL 应答
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">请求包</param>
        /// <returns></returns>
        Task<ResultPacket> PubCompAsync(ISocketClient client, PubRelPacket packet);
        /// <summary>
        /// 分发数据
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="packet">数据包</param>
        /// <returns></returns>
        Task PublishAsync(ISocketClient client, PublishPacket packet);
        /// <summary>
        /// 发送保存分发数据
        /// </summary>
        /// <param name="client">MQTT客户端</param>
        /// <param name="topicFilter">主题过滤器</param>
        /// <returns></returns>
        Task<bool> SendPublishAsync(ISocketClient client, TopicFilter topicFilter);
        /// <summary>
        /// 保存分发数据
        /// </summary>
        /// <param name="packet">分发数据</param>
        /// <returns></returns>
        Task<bool> SavePublishAsync(PublishPacket packet);
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="packet">消息包</param>
        /// <param name="client">客户端</param>
        /// <returns></returns>
        Task<bool> SendAsync(MqttPacket packet, ISocketClient client = null);
        /// <summary>
        /// 添加凭证
        /// </summary>
        /// <param name="credential">用户凭证</param>
        void AddCredential(IMqttServerCredential credential);
        /// <summary>
        /// 添加凭证
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="password">密码</param>
        void AddCredential(string userName, string password);
        /// <summary>
        /// 添加凭证
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="password">密码</param>
        /// <param name="allowClientIp">允许IP</param>
        void AddCredential(string userName, string password, string allowClientIp);
        /// <summary>
        /// 添加凭证
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="password">密码</param>
        /// <param name="allowClientIp">允许IP</param>
        void AddCredential(string userName, string password, IList<string> allowClientIp);
        /// <summary>
        /// 添加凭证
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="password">密码</param>
        /// <param name="allowClientIp">允许IP</param>
        void AddCredential(string userName, string password, params string[] allowClientIp);
        /// <summary>
        /// 设置当前账号允许IP
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="allowClientIp">允许IP</param>
        void AddCredentialAllowClientIp(string userName, string allowClientIp);
        /// <summary>
        /// 移除凭证
        /// </summary>
        /// <param name="userName">账号</param>
        /// <returns></returns>
        bool RemoveCredential(string userName);
        /// <summary>
        /// 释放
        /// </summary>
        void Dispose();
        #endregion
    }
}