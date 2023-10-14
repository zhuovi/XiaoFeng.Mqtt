using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-28 15:19:30                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Internal
{
    /// <summary>
    /// Will Message的Payload格式定义
    /// </summary>
    public enum MqttPayloadFormatIndicator
    {
        /// <summary>
        /// 未定义
        /// </summary>
        Unspecified = 0x00,
        /// <summary>
        /// 字符数据
        /// </summary>
        CharacterData = 0x01
    }
}