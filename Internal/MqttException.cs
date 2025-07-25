using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-08 14:56:19                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt
{
    /// <summary>
    /// Mqtt异常类
    /// </summary>
    public class MqttException : Exception
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MqttException()
        {

        }
        /// <summary>
        /// 初如化一个新实例
        /// </summary>
        /// <param name="message">消息</param>
        public MqttException(string message):base(message) { }
        /// <summary>
        /// 初如化一个新实例
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="ex">异常</param>
        public MqttException(string message, Exception ex) : base(message, ex) { }
        #endregion

        #region 属性

        #endregion

        #region 方法

        #endregion
    }
}