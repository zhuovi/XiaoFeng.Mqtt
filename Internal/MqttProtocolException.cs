using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-07 16:44:23                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Internal
{
    /// <summary>
    /// 协议版本异常
    /// </summary>
    public class MqttProtocolException : Exception
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MqttProtocolException()
        {

        }
        ///<inheritdoc/>
        public MqttProtocolException(string message) : base(message)
        {

        }
        #endregion

        #region 属性

        #endregion

        #region 方法

        #endregion
    }
}