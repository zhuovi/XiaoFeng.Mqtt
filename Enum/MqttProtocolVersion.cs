using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-06-16 17:20:40                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt
{
    /// <summary>
    /// MQTT 版本
    /// </summary>
    public enum MqttProtocolVersion : byte
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// MQTT 3.1.0
        /// </summary>
        V310 = 0x03,
        /// <summary>
        /// MQTT 3.1.1
        /// </summary>
        V311 = 0x04,
        /// <summary>
        /// MQTT 5.0
        /// </summary>
        V500 = 0x05
    }
}