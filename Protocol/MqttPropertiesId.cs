using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-28 09:05:37                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Protocol
{
    /// <summary>
    /// 用户属性标识
    /// </summary>
    public enum MqttPropertiesId
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 有效负载格式指示器
        /// </summary>
        PayloadFormatIndicator = 0x01,
        /// <summary>
        /// 消息到期间隔
        /// </summary>
        MessageExpiryInterval = 0x02,
        /// <summary>
        /// 内容类型
        /// </summary>
        ContentType = 0x03,
        /// <summary>
        /// 响应主题
        /// </summary>
        ResponseTopic = 0x08,
        /// <summary>
        /// 相关数据
        /// </summary>
        CorrelationData = 0x09,
        /// <summary>
        /// 订阅标识符
        /// </summary>
        SubscriptionIdentifier = 0x0B,
        /// <summary>
        /// 会话到期
        /// </summary>
        SessionExpiryInterval = 0x11,
        /// <summary>
        /// 指定的客户端标识符
        /// </summary>
        AssignedClientIdentifier = 0x12,
        /// <summary>
        /// 服务器保持活动
        /// </summary>
        ServerKeepAlive = 0x13,
        /// <summary>
        /// 身份验证方法
        /// </summary>
        AuthenticationMethod = 0x15,
        /// <summary>
        /// 身份验证数据
        /// </summary>
        AuthenticationData = 0x16,
        /// <summary>
        /// 请求问题信息
        /// </summary>
        RequestProblemInformation = 0x17,
        /// <summary>
        /// 将延迟间隔
        /// </summary>
        WillDelayInterval = 0x18,
        /// <summary>
        /// 请求响应信息
        /// </summary>
        RequestResponseInformation = 0x19,
        /// <summary>
        /// 响应信息
        /// </summary>
        ResponseInformation = 0x1A,
        /// <summary>
        /// 服务器参考
        /// </summary>
        ServerReference = 0x1C,
        /// <summary>
        /// 原因字符串
        /// </summary>
        ReasonString = 0x1F,
        /// <summary>
        /// 接收最大值
        /// </summary>
        ReceiveMaximum = 0x21,
        /// <summary>
        /// 主题别名最大值
        /// </summary>
        TopicAliasMaximum = 0x22,
        /// <summary>
        /// 主题别名
        /// </summary>
        TopicAlias = 0x23,
        /// <summary>
        /// 最大QoS
        /// </summary>
        MaximumQoS = 0x24,
        /// <summary>
        /// 保留可用
        /// </summary>
        RetainAvailable = 0x25,
        /// <summary>
        /// 用户属性
        /// </summary>
        UserProperty = 0x26,
        /// <summary>
        /// 最大数据包大小
        /// </summary>
        MaximumPacketSize = 0x27,
        /// <summary>
        /// 通配符订阅可用
        /// </summary>
        WildcardSubscriptionAvailable = 0x28,
        /// <summary>
        /// 订阅标识符可用
        /// </summary>
        SubscriptionIdentifiersAvailable = 0x29,
        /// <summary>
        /// 共享订阅可用
        /// </summary>
        SharedSubscriptionAvailable = 0x2A
    }
}