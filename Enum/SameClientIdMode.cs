using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using XiaoFeng.Mqtt.Server;

/****************************************************************
*  Copyright © (2025) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2025-01-02 11:51:09                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt
{
    /// <summary>
    /// 客户端ID相同处理模式
    /// </summary>
    public enum SameClientIdMode
    {
        /// <summary>
        /// 拒绝连接
        /// </summary>
        [Description("拒绝连接")]
        Refused = 0,
        /// <summary>
        /// 断开现有连接
        /// </summary>
        [Description("断开现有连接")]
        DisconnectExisting = 1,
        /// <summary>
        /// 重置客户端Id
        /// </summary>
        [Description("重置客户端Id")]
        ResetClientId = 2
    }
}