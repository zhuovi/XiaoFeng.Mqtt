using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-06-16 15:24:29                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt
{
    /*
     *                  控制报文的类型
     *   ------------------------------------------------------------------------------               
     *   名字	        值	    报文流动方向	    描述
     *   ------------------------------------------------------------------------------
     *   Reserved	    0	    禁止	            保留
     *   CONNECT        1	    客户端到服务端	    客户端请求连接服务端
     *   CONNACK	    2	    服务端到客户端	    连接报文确认
     *   PUBLISH	    3	    两个方向都允许	    发布消息
     *   PUBACK	        4	    两个方向都允许	    QoS 1消息发布收到确认
     *   PUBREC	        5	    两个方向都允许	    发布收到（保证交付第一步）
     *   PUBREL	        6	    两个方向都允许	    发布释放（保证交付第二步）
     *   PUBCOMP	    7	    两个方向都允许	    QoS 2消息发布完成（保证交互第三步）
     *   SUBSCRIBE	    8	    客户端到服务端	    客户端订阅请求
     *   SUBACK	        9	    服务端到客户端	    订阅请求报文确认
     *   UNSUBSCRIBE    10	    客户端到服务端	    客户端取消订阅请求
     *   UNSUBACK	    11	    服务端到客户端	    取消订阅报文确认
     *   PINGREQ	    12	    客户端到服务端	    心跳请求
     *   PINGRESP	    13	    服务端到客户端	    心跳响应
     *   DISCONNECT	    14	    客户端到服务端	    客户端断开连接
     *   Reserved	    15	    禁止	            保留
     */
    /// <summary>
    /// 报文类型(MQTT Control Packet type)
    /// </summary>
    [Flags]
    public enum PacketType : byte
    {
        /// <summary>
        /// 保留
        /// </summary>
        //RESERVED = 0,
        /// <summary>
        /// 客户端请求连接服务端
        /// </summary>
        /// <remarks>客户端到服务端</remarks>
        CONNECT = 1,
        /// <summary>
        /// 连接报文确认
        /// </summary>
        /// <remarks>服务端到客户端</remarks>
        CONNACK = 2,
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <remarks>两个方向都允许</remarks>
        PUBLISH = 3,
        /// <summary>
        /// QoS 1消息发布收到确认
        /// </summary>
        /// <remarks>两个方向都允许</remarks>
        PUBACK = 4,
        /// <summary>
        /// 发布收到（保证交付第一步）
        /// </summary>
        /// <remarks>两个方向都允许</remarks>
        PUBREC = 5,
        /// <summary>
        /// 发布释放（保证交付第二步）
        /// </summary>
        /// <remarks>两个方向都允许</remarks>
        PUBREL = 6,
        /// <summary>
        /// QoS 2消息发布完成（保证交互第三步）
        /// </summary>
        /// <remarks>两个方向都允许</remarks>
        PUBCOMP = 7,
        /// <summary>
        /// 客户端订阅请求
        /// </summary>
        /// <remarks>两个方向都允许	</remarks>
        SUBSCRIBE = 8,
        /// <summary>
        /// 订阅请求报文确认
        /// </summary>
        /// <remarks>服务端到客户端</remarks>
        SUBACK = 9,
        /// <summary>
        /// 客户端取消订阅请求
        /// </summary>
        /// <remarks>客户端到服务端</remarks>
        UNSUBSCRIBE = 10,
        /// <summary>
        /// 取消订阅报文确认
        /// </summary>
        /// <remarks>服务端到客户端</remarks>
        UNSUBACK = 11,
        /// <summary>
        /// 心跳请求
        /// </summary>
        /// <remarks>客户端到服务端</remarks>
        PINGREQ = 12,
        /// <summary>
        /// 心跳响应
        /// </summary>
        /// <remarks>服务端到客户端</remarks>
        PINGRESP = 13,
        /// <summary>
        /// 客户端断开连接
        /// </summary>
        /// <remarks>两个方向都允许	</remarks>
        DISCONNECT = 14,
        /// <summary>
        /// 认证信息交换(V5.0使用)
        /// </summary>
        /// <remarks>两个方向都允许	</remarks>
        AUTH = 15,
    }
}