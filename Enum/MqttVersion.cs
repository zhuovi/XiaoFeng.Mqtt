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
namespace XiaoFeng.Mqtt.Enum
{
    /// <summary>
    /// MQTT 版本
    /// </summary>
    public enum MqttVersion : byte
    {
        /// <summary>
        /// MQTT 3.1
        /// </summary>
        MQTT_3_1 = 0x03,
        /// <summary>
        /// MQTT 3.1.1
        /// </summary>
        MQTT_3_1_1 = 0x04,
        /// <summary>
        /// MQTT 5.0
        /// </summary>
        MQTT_5 = 0x05
    }
}