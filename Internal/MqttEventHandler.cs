using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using XiaoFeng.Mqtt.Packets;
using XiaoFeng.Mqtt.Server;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-11 20:39:24                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt
{
    /// <summary>
    /// MQTT客户端连接MQTT服务端事件
    /// </summary>
    /// <param name="client">MQTT客户端</param>
    public delegate void MqttClientConnectedEventHandler(IMqttClient client);
    /// <summary>
    /// MQTT客户端错误事件
    /// </summary>
    /// <param name="client">MQTT客户端</param>
    /// <param name="message">错误信息</param>
    public delegate void MqttClientErrorEventHandler(IMqttClient client, string message);
    /// <summary>
    /// MQTT客户端接收事件
    /// </summary>
    /// <param name="packet">数据包</param>
    public delegate void MqttClientMessageEventHandler(ResultPacket packet);
    /// <summary>
    /// MQTT客户端接收所订阅的消息事件
    /// </summary>
    /// <param name="packet">消息包</param>
    public delegate void MqttClientPublishMessageEventHandler(PublishPacket packet);
    /// <summary>
    /// MQTT客户端断开事件
    /// </summary>
    /// <param name="client">数据包</param>
    public delegate void MqttClientDisconnectedEventHandler(IMqttClient client);
    /// <summary>
    /// MQTT客户端启动事件
    /// </summary>
    /// <param name="client">数据包</param>
    public delegate void MqttClientStartedEventHandler(IMqttClient client);
    /// <summary>
    /// MQTT客户端停止事件
    /// </summary>
    /// <param name="client">数据包</param>
    public delegate void MqttClientStopedEventHandler(IMqttClient client);

    /// <summary>
    /// MQTT服务端启动事件
    /// </summary>
    /// <param name="server">MQTT服务端</param>
    public delegate void MqttServerStaredEventHandler(IMqttServer server);
    /// <summary>
    /// MQTT服务端停止事件
    /// </summary>
    /// <param name="server">MQTT服务端</param>
    public delegate void MqttServerStopedEventHandler(IMqttServer server);
    /// <summary>
    /// MQTT客户端新连接事件
    /// </summary>
    /// <param name="client">MQTT客户端</param>
    public delegate void MqttServerConnectedEventHandler(ISocketClient client);
    /// <summary>
    /// MQTT客户端断开事件
    /// </summary>
    /// <param name="client">MQTT客户端</param>
    public delegate void MqttServerDisconnectedEventHandler(ISocketClient client);
    /// <summary>
    /// MQTT服务端出错事件
    /// </summary>
    /// <param name="server">MQTT服务端</param>
    /// <param name="message">消息</param>
    public delegate void MqttServerErrorEventHandler(IMqttServer server, string message);
    /// <summary>
    /// MQTT服务端消息分发事件
    /// </summary>
    /// <param name="server">MQTT服务端</param>
    /// <param name="publishClient">发送客户端</param>
    /// <param name="packet">消息包</param>
    /// <param name="distributeClients">分发客户端</param>
    public delegate void MqttServerDistributeEventHandler(IMqttServer server, ISocketClient publishClient, PublishPacket packet, List<DistributeData> distributeClients);
    /// <summary>
    /// MQTT客户端错误事件
    /// </summary>
    /// <param name="client">MQTT客户端</param>
    /// <param name="endPoint">客户端终结点</param>
    /// <param name="message">消息</param>
    public delegate void MqttServerClientErrorEventHandler(ISocketClient client, IPEndPoint endPoint, string message);
    /// <summary>
    /// MQTT服务端接收消息事件
    /// </summary>
    /// <param name="client">MQTT客户端</param>
    /// <param name="packet">结果包</param>
    public delegate void MqttServerMessageEventHandler(ISocketClient client, ResultPacket packet);
}