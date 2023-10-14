using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-11 09:58:45                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Internal
{
    /// <summary>
    /// 取消订阅结果
    /// </summary>
    public class UnsubscribeResult
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public UnsubscribeResult()
        {

        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="topic">主题</param>
        public UnsubscribeResult(string topic) : this(topic, string.Empty) { }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="message">消息</param>
        public UnsubscribeResult(string topic, string message)
        {
            this.Topic = topic.Trim(new char[] { '/', ' ' });
            if (message.IsNullOrEmpty())
                this.Status = true;
            else
            {
                this.Status = false;
                this.Message = message;
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 主题
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}