using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-09 15:59:32                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt
{
    /// <summary>
    /// 已保存的消息发送类型
    /// </summary>
    public enum RetainHandling
    {
        /// <summary>
        /// 在订阅时，发送已保存的消息
        /// </summary>
        SendAtSubscribe = 0,
        /// <summary>
        /// 在订阅时且当前不存在该订阅时（初次订阅），发送已保存的消息
        /// </summary>
        SendAtSubscribeIfNewSubscriptionOnly = 1,
        /// <summary>
        /// 在订阅时，不发送已保存的消息
        /// </summary>
        DoNotSendOnSubscribe = 2
    }
}