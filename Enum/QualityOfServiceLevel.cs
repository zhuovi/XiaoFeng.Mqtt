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
    /// <summary>
    /// 服务质量等级
    /// </summary>
    [Flags]
    public enum QualityOfServiceLevel : byte
    {
        /// <summary>
        /// 最多分发一次
        /// </summary>
        /// <remarks>
        /// QOS Level 0 - 消息的分发依赖于底层网络的能力。接收者不会发送响应，发送者也不会重试。消息可能送达一次也可能根本没送达。
        /// </remarks>
        AtMostOnce = 0x00,
        /// <summary>
        /// 至少分发一次        
        /// </summary>
        /// <remarks>
        /// QOS Level 1 - 服务质量确保消息至少送达一次。QoS 1的PUBLISH报文的可变报头中包含一个报文标识符，需要PUBACK报文确认。
        /// </remarks>
        AtLeastOnce = 0x01,
        /// <summary>
        /// 仅分发一次
        /// </summary>
        /// <remarks>
        /// QOS Level 2 - 这是最高等级的服务质量，消息丢失和重复都是不可接受的。使用这个服务质量等级会有额外的开销。
        /// QoS 2的消息可变报头中有报文标识符。
        /// </remarks>
        ExactlyOnce = 0x02,
        /// <summary>
        /// 保留
        /// </summary>
        Reserved = 0x03,
        /// <summary>
        /// 失败
        /// </summary>
        Failure = 0x80
    }
}