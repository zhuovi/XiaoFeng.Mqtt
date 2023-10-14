using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-07 17:32:40                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt
{
    /// <summary>
    /// 载荷格式指示符
    /// </summary>
    /// <remarks>
    /// <para>Type 为 1（0x01），Value 为单个 Byte。取值为 0 或者 1：</para>
    /// <para>0：表示 Payload 为未定义 bytes，等价于不存在该 Property；</para>
    /// <para>1：表示 Payload 为 UTF-8 编码字符串，满足[Unicode]和 RFC3629 规范。</para>
    /// <para>Server 必 须 不 加 改 变 的 给 Application Message 的接收 者 （ 订 阅 者 ） 发 送 Payload Format Indicator[MQTT-3.3.2-4]。接收者可以检查 Payload 的格式是否与 Payload Format Indicator 一致，如果不一致，发送 PUBACK、PUBREC、DISCONNECT 控制包，并携带 Reason Code 0x99（Payload format invalid）。参考 5.4.9 章节，关于验证 Payload 格式相关的安全问题。</para>
    /// </remarks>
    public enum MqttPayloadFormatIndicator
    {
        /// <summary>
        /// 未定义bytes
        /// </summary>
        Unspecified = 0,
        /// <summary>
        /// 有字符数据
        /// </summary>
        CharacterData = 1
    }
}