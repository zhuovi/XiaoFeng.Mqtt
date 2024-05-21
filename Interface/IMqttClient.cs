using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Mqtt.Client;
using XiaoFeng.Mqtt.Internal;
using XiaoFeng.Mqtt.Packets;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-27 15:04:19                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt
{
    /// <summary>
    /// Mqtt客户端接口
    /// </summary>
    public interface IMqttClient
    {
        #region 属性
        /// <summary>
        /// 是否已连接
        /// </summary>
        bool Connected { get; }
        /// <summary>
        /// 是否激活
        /// </summary>
        bool Active { get; }
        /// <summary>
        /// 我的订阅
        /// </summary>
        ConcurrentDictionary<string, TopicFilter> TopicFilters { get; set; }
        /// <summary>
        /// 客户端配置
        /// </summary>
        MqttClientOptions ClientOptions { get; set; }
        #endregion

        #region 事件
        /// <summary>
        /// 连接
        /// </summary>
        event MqttClientConnectedEventHandler OnConnected;
        /// <summary>
        /// 出错
        /// </summary>
        event MqttClientErrorEventHandler OnError;
        /// <summary>
        /// 消息
        /// </summary>
        event MqttClientMessageEventHandler OnMessage;
        /// <summary>
        /// MQTT客户端接收所订阅的消息事件
        /// </summary>
        event MqttClientPublishMessageEventHandler OnPublishMessage;
        /// <summary>
        /// 断开
        /// </summary>
        event MqttClientDisconnectedEventHandler OnDisconnected;
        /// <summary>
        /// 启动
        /// </summary>
        event MqttClientStartedEventHandler OnStarted;
        /// <summary>
        /// 停止
        /// </summary>
        event MqttClientStopedEventHandler OnStoped;
        #endregion

        #region 方法
        /// <summary>
        /// 连接服务端
        /// </summary>
        /// <returns></returns>
        Task<ConnectActPacket> ConnectAsync();
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        Task<bool> DisconnectAsync();
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="packet">断开包</param>
        /// <returns></returns>
        Task<bool> DisconnectAsync(DisconnectPacket packet);
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="qos">服务质量等级</param>
        /// <returns></returns>
        Task<SubscribeResult> SubscributeAsync(string topic, QualityOfServiceLevel qos = QualityOfServiceLevel.AtLeastOnce);
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topicFilter">订阅过滤器</param>
        /// <returns></returns>
        Task<SubscribeResult> SubscributeAsync(TopicFilter topicFilter);
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topics">主题</param>
        /// <returns></returns>
        Task<IList<SubscribeResult>> SubscributeAsync(ICollection<KeyValuePair<string, QualityOfServiceLevel>> topics);
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topicFilters">订阅过滤器</param>
        /// <returns></returns>
        Task<IList<SubscribeResult>> SubscributeAsync(IEnumerable<TopicFilter> topicFilters);
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="qos">服务质量等级</param>
        /// <returns></returns>
        Task<UnsubscribeResult> UnsubscributeAsync(string topic, QualityOfServiceLevel qos = QualityOfServiceLevel.AtLeastOnce);
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topicFilter">订阅过滤器</param>
        /// <returns></returns>
        Task<UnsubscribeResult> UnsubscributeAsync(TopicFilter topicFilter);
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topics">主题</param>
        /// <returns></returns>
        Task<IList<UnsubscribeResult>> UnsubscributeAsync(ICollection<KeyValuePair<string, QualityOfServiceLevel>> topics);
        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="topicFilters">订阅主题</param>
        /// <returns></returns>
        Task<IList<UnsubscribeResult>> UnsubscributeAsync(IEnumerable<TopicFilter> topicFilters);
        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="data">数据</param>
        /// <param name="qos">服务质量等级</param>
        /// <returns></returns>
        Task<bool> PublishAsync(string topic, object data, QualityOfServiceLevel qos = QualityOfServiceLevel.AtLeastOnce);
        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="packet">发布包</param>
        /// <returns></returns>
        Task<bool> PublishAsync(PublishPacket packet);
        /// <summary>
        /// 发送Ping
        /// </summary>
        /// <returns></returns>
        Task<bool> PingAsync();
        /// <summary>
        /// 认证
        /// </summary>
        /// <param name="packet">认证包</param>
        /// <returns></returns>
        Task<bool> AuthAsync(AuthPacket packet);
        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
        /// <summary>
        /// 注销
        /// </summary>
        void Dispose();
        #endregion
    }
}