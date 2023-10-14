using System;
using System.Collections.Generic;
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
        SUCCESS = 0x00,
        /// <summary>
        /// 授权的QoS 1
        /// </summary>
        /// <remarks>
        /// <term>报文</term> SUBACK
        /// </remarks>
        QOS1 = 0x01,
        /// <summary>
        /// 授权的QoS 2
        /// </summary>
        /// <remarks>
        /// <term>报文</term> SUBACK
        /// </remarks>
        QOS2 = 0x02,
        /// <summary>
        /// 包含遗嘱的断开
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        DISCONNECT_WITH_WILL_MESSAGE = 0x04,
        /// <summary>
        /// 无匹配订阅
        /// </summary>
        /// <remarks>
        /// <term>报文</term> PUBACK, PUBREC
        /// </remarks>
        NO_MATCHING_SUBSCRIBERS = 0x10,
        /// <summary>
        /// 订阅不存在
        /// </summary>
        /// <remarks>
        /// <term>报文</term> UNSUBACK
        /// </remarks>
        NO_SUBSCRIPTION_EXISTED = 0x11,
        /// <summary>
        /// 继续认证
        /// </summary>
        /// <remarks>
        /// <term>报文</term> AUTH
        /// </remarks>
        CONTINUE_AUTHENTICATION = 0x18,
        /// <summary>
        /// 重新认证
        /// </summary>
        /// <remarks>
        /// <term>报文</term> AUTH
        /// </remarks>
        RECERTIFICATE = 0x19,
        /// <summary>
        /// 未指明的错误
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, PUBACK, PUBREC, SUBACK, UNSUBACK, DISCONNECT
        /// </remarks>
        UNSPECIFIED_ERROR = 0x80,
        /// <summary>
        /// 无效报文
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        MALFORMED_PACKET = 0x81,
        /// <summary>
        /// 协议错误
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        PROTOCOL_ERROR = 0x82,
        /// <summary>
        /// 实现错误
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, PUBACK, PUBREC, SUBACK, UNSUBACK, DISCONNECT
        /// </remarks>
        IMPLEMENTATION_SPECIFIC_ERROR = 0x83,
        /// <summary>
        /// 协议版本不支持
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK
        /// </remarks>
        UNSUPPORTED_PROTOCOL_VERSION = 0x84,
        /// <summary>
        /// 客户标识符无效
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK
        /// </remarks>
        CLIENT_IDENTIFIER_NOT_VALID = 0x85,
        /// <summary>
        /// 用户名密码错误
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK
        /// </remarks>
        BAD_USERNAME_OR_PASSWORD = 0x86,
        /// <summary>
        /// 未授权
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, PUBACK, PUBREC, SUBACK, UNSUBACK, DISCONNECT
        /// </remarks>
        NOT_AUTHORIZED = 0x87,
        /// <summary>
        /// 服务端不可用
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK
        /// </remarks>
        SERVER_UNAVAILABLE = 0x88,
        /// <summary>
        /// 服务端正忙
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        SERVER_BUSY = 0x89,
        /// <summary>
        /// 禁止
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK
        /// </remarks>
        BANNED = 0x8A,
        /// <summary>
        /// 服务端关闭中	
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        SERVER_SHUTTING_DOWN = 0x8B,
        /// <summary>
        /// 错误的认证方法
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        BAD_AUTHENTICATION_METHOND = 0x8C,
        /// <summary>
        /// 保持超时
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        KEEP_ALIVET_TIMEOUT = 0x8D,
        /// <summary>
        /// 会话被接管
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        SESSION_TAKEN_OVER = 0x8E,
        /// <summary>
        /// 主题过滤器无效
        /// </summary>
        /// <remarks>
        /// <term>报文</term> SUBACK, UNSUBACK, DISCONNECT
        /// </remarks>
        TOPIC_FILTER_INVALID = 0x8F,
        /// <summary>
        /// 主题名无效
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, PUBACK, PUBREC, DISCONNECT
        /// </remarks>
        TOPIC_NAME_INVALID = 0x90,
        /// <summary>
        /// 报文标识符已被占用	
        /// </summary>
        /// <remarks>
        /// <term>报文</term> PUBACK, PUBREC, SUBACK, UNSUBACK
        /// </remarks>
        PACKET_IDENTIFIER_IN_USE = 0x91,
        /// <summary>
        /// 报文标识符无效
        /// </summary>
        /// <remarks>
        /// <term>报文</term> PUBREL, PUBCOMP
        /// </remarks>
        PACKET_IDENTIFIER_NOT_FOUND = 0x92,
        /// <summary>
        /// 接收超出最大数量
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        RECEIVE_MAXIMUM_EXCEEDED = 0x93,
        /// <summary>
        /// 主题别名无效
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        TOPIC_ALIAS_INVALID = 0x94,
        /// <summary>
        /// 报文过长
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        PACKET_TOO_LARGE = 0x95,
        /// <summary>
        /// 消息太过频繁
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        MESSAGE_RATE_TOO_HIGH = 0x96,
        /// <summary>
        /// 超出配额
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, PUBACK, PUBREC, SUBACK, DISCONNECT
        /// </remarks>
        QUOTA_EXCEEDED = 0x97,
        /// <summary>
        /// 管理行为
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        ADMINISTRATIVE_ACTION = 0x98,
        /// <summary>
        /// 载荷格式无效
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, PUBACK, PUBREC, DISCONNECT
        /// </remarks>
        PAYLOAD_FORMAT_INVALID = 0x99,
        /// <summary>
        /// 不支持保留
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        RETAIN_NOT_SUPPORTED = 0x9A,
        /// <summary>
        /// 不支持的QoS等级
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        QOS_NOT_SUPPORTED = 0x9B,
        /// <summary>
        /// （临时）使用其他服务端	
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        USE_ANOTHER_SERVER = 0x9C,
        /// <summary>
        /// 服务端已（永久）移动
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        SERVER_MOVED = 0x9D,
        /// <summary>
        /// 不支持共享订阅
        /// </summary>
        /// <remarks>
        /// <term>报文</term> SUBACK, DISCONNECT
        /// </remarks>
        SHARED_SUBSCRIPTION_NOT_SUPPORTED = 0x9E,
        /// <summary>
        /// 超出连接速率限制
        /// </summary>
        /// <remarks>
        /// <term>报文</term> CONNACK, DISCONNECT
        /// </remarks>
        CONNECTION_RATE_EXCEEDED = 0x9F,
        /// <summary>
        /// 最大连接时间
        /// </summary>
        /// <remarks>
        /// <term>报文</term> DISCONNECT
        /// </remarks>
        MAXIMUM_CONNECT_TIME = 0xA0,
        /// <summary>
        /// 不支持订阅标识符
        /// </summary>
        /// <remarks>
        /// <term>报文</term> SUBACK, DISCONNECT
        /// </remarks>
        SUBCRIPTION_IDENTIFIERS_NOT_SUPPORTED = 0xA1,
        /// <summary>
        /// 不支持通配符订阅
        /// </summary>
        /// <remarks>
        /// <term>报文</term> SUBACK, DISCONNECT
        /// </remarks>
        WILDCARD_SUBSCRIPTIONS_NOT_SUPPORTED = 0xA2,
    }
}