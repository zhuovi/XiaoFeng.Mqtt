using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-27 10:37:15                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt
{
    /// <summary>
    /// 原因码
    /// </summary>
    public enum ReasonCode
    {
        /// <summary>
        /// 成功,正常断开,授权的QoS 0
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, PUBACK, PUBREC, PUBREL, PUBCOMP, UNSUBACK, AUTH, DISCONNECT,SUBACK
        /// </remarks>
        [Description("成功,正常断开,授权的QoS 0")]
        SUCCESS = 0x00,
        /// <summary>
        /// 授权的QoS 1
        /// </summary>
        /// <remarks>
        /// <term>报文</term> SUBACK
        /// </remarks>
        [Description("授权的QoS 1")]
        QOS1 = 0x01,
        /// <summary>
        /// 授权的QoS 2
        /// </summary>
        /// <remarks>
        /// <term>报文</term> SUBACK
        /// </remarks>
        [Description("授权的QoS 2")]
        QOS2 = 0x02,
        /// <summary>
        /// 包含遗嘱的断开
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        [Description("包含遗嘱的断开")]
        DISCONNECT_WITH_WILL_MESSAGE = 0x04,
        /// <summary>
        /// 无匹配订阅
        /// </summary>
        /// <remarks>
        /// <term>报文</term> PUBACK, PUBREC
        /// </remarks>
        [Description("无匹配订阅")]
        NO_MATCHING_SUBSCRIBERS = 0x10,
        /// <summary>
        /// 订阅不存在
        /// </summary>
        /// <remarks>
        /// <term>报文</term> UNSUBACK
        /// </remarks>
        [Description("订阅不存在")] 
        NO_SUBSCRIPTION_EXISTED = 0x11,
        /// <summary>
        /// 继续认证
        /// </summary>
        /// <remarks>
        /// <term>报文</term> AUTH
        /// </remarks>
        [Description("继续认证")] 
        CONTINUE_AUTHENTICATION = 0x18,
        /// <summary>
        /// 重新认证
        /// </summary>
        /// <remarks>
        /// <term>报文</term> AUTH
        /// </remarks>
        [Description("重新认证")] 
        RECERTIFICATE = 0x19,
        /// <summary>
        /// 未指明的错误
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, PUBACK, PUBREC, SUBACK, UNSUBACK, DISCONNECT
        /// </remarks>
        [Description("未指明的错误")] 
        UNSPECIFIED_ERROR = 0x80,
        /// <summary>
        /// 无效报文
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        [Description("无效报文")] 
        MALFORMED_PACKET = 0x81,
        /// <summary>
        /// 协议错误
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        [Description("协议错误")] 
        PROTOCOL_ERROR = 0x82,
        /// <summary>
        /// 实现错误
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, PUBACK, PUBREC, SUBACK, UNSUBACK, DISCONNECT
        /// </remarks>
        [Description("实现错误")] 
        IMPLEMENTATION_SPECIFIC_ERROR = 0x83,
        /// <summary>
        /// 协议版本不支持
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK
        /// </remarks>
        [Description("协议版本不支持")] 
        UNSUPPORTED_PROTOCOL_VERSION = 0x84,
        /// <summary>
        /// 客户标识符无效
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK
        /// </remarks>
        [Description("客户标识符无效")] 
        CLIENT_IDENTIFIER_NOT_VALID = 0x85,
        /// <summary>
        /// 用户名密码错误
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK
        /// </remarks>
        [Description("用户名密码错误")] 
        BAD_USERNAME_OR_PASSWORD = 0x86,
        /// <summary>
        /// 未授权
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, PUBACK, PUBREC, SUBACK, UNSUBACK, DISCONNECT
        /// </remarks>
        [Description("未授权")] 
        NOT_AUTHORIZED = 0x87,
        /// <summary>
        /// 服务端不可用
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK
        /// </remarks>
        [Description("服务端不可用")] 
        SERVER_UNAVAILABLE = 0x88,
        /// <summary>
        /// 服务端正忙
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        [Description("服务端正忙")] 
        SERVER_BUSY = 0x89,
        /// <summary>
        /// 禁止
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK
        /// </remarks>
        [Description("禁止")] 
        BANNED = 0x8A,
        /// <summary>
        /// 服务端关闭中	
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        [Description("服务端关闭中")] 
        SERVER_SHUTTING_DOWN = 0x8B,
        /// <summary>
        /// 错误的认证方法
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        [Description("错误的认证方法")] 
        BAD_AUTHENTICATION_METHOND = 0x8C,
        /// <summary>
        /// 保持超时
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        [Description("保持超时")] 
        KEEP_ALIVET_TIMEOUT = 0x8D,
        /// <summary>
        /// 会话被接管
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        [Description("会话被接管")] 
        SESSION_TAKEN_OVER = 0x8E,
        /// <summary>
        /// 主题过滤器无效
        /// </summary>
        /// <remarks>
        /// <term>报文</term> SUBACK, UNSUBACK, DISCONNECT
        /// </remarks>
        [Description("主题过滤器无效")] 
        TOPIC_FILTER_INVALID = 0x8F,
        /// <summary>
        /// 主题名无效
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, PUBACK, PUBREC, DISCONNECT
        /// </remarks>
        [Description("主题名无效")] 
        TOPIC_NAME_INVALID = 0x90,
        /// <summary>
        /// 报文标识符已被占用	
        /// </summary>
        /// <remarks>
        /// <term>报文</term> PUBACK, PUBREC, SUBACK, UNSUBACK
        /// </remarks>
        [Description("报文标识符已被占用")] 
        PACKET_IDENTIFIER_IN_USE = 0x91,
        /// <summary>
        /// 报文标识符无效
        /// </summary>
        /// <remarks>
        /// <term>报文</term> PUBREL, PUBCOMP
        /// </remarks>
        [Description("报文标识符无效")] 
        PACKET_IDENTIFIER_NOT_FOUND = 0x92,
        /// <summary>
        /// 接收超出最大数量
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        [Description("接收超出最大数量")] 
        RECEIVE_MAXIMUM_EXCEEDED = 0x93,
        /// <summary>
        /// 主题别名无效
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        [Description("主题别名无效")] 
        TOPIC_ALIAS_INVALID = 0x94,
        /// <summary>
        /// 报文过长
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        [Description("报文过长")] 
        PACKET_TOO_LARGE = 0x95,
        /// <summary>
        /// 消息太过频繁
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        [Description("消息太过频繁")] 
        MESSAGE_RATE_TOO_HIGH = 0x96,
        /// <summary>
        /// 超出配额
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, PUBACK, PUBREC, SUBACK, DISCONNECT
        /// </remarks>
        [Description("超出配额")] 
        QUOTA_EXCEEDED = 0x97,
        /// <summary>
        /// 管理行为
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        [Description("管理行为")] 
        ADMINISTRATIVE_ACTION = 0x98,
        /// <summary>
        /// 载荷格式无效
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, PUBACK, PUBREC, DISCONNECT
        /// </remarks>
        [Description("载荷格式无效")] 
        PAYLOAD_FORMAT_INVALID = 0x99,
        /// <summary>
        /// 不支持保留
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        [Description("不支持保留")] 
        RETAIN_NOT_SUPPORTED = 0x9A,
        /// <summary>
        /// 不支持的QoS等级
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        [Description("不支持的QoS等级")] 
        QOS_NOT_SUPPORTED = 0x9B,
        /// <summary>
        /// （临时）使用其他服务端	
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        [Description("（临时）使用其他服务端")] 
        USE_ANOTHER_SERVER = 0x9C,
        /// <summary>
        /// 服务端已（永久）移动
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        [Description("服务端已（永久）移动")] 
        SERVER_MOVED = 0x9D,
        /// <summary>
        /// 不支持共享订阅
        /// </summary>
        /// <remarks>
        /// <term>报文</term> SUBACK, DISCONNECT
        /// </remarks>
        [Description("不支持共享订阅")] 
        SHARED_SUBSCRIPTION_NOT_SUPPORTED = 0x9E,
        /// <summary>
        /// 超出连接速率限制
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        [Description("超出连接速率限制")] 
        CONNECTION_RATE_EXCEEDED = 0x9F,
        /// <summary>
        /// 最大连接时间
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        [Description("最大连接时间")] 
        MAXIMUM_CONNECT_TIME = 0xA0,
        /// <summary>
        /// 不支持订阅标识符
        /// </summary>
        /// <remarks>
        /// <term>报文</term> SUBACK, DISCONNECT
        /// </remarks>
        [Description("不支持订阅标识符")] 
        SUBCRIPTION_IDENTIFIERS_NOT_SUPPORTED = 0xA1,
        /// <summary>
        /// 不支持通配符订阅
        /// </summary>
        /// <remarks>
        /// <term>报文</term> SUBACK, DISCONNECT
        /// </remarks>
        [Description("不支持通配符订阅")] 
        WILDCARD_SUBSCRIPTIONS_NOT_SUPPORTED = 0xA2,
    }
}