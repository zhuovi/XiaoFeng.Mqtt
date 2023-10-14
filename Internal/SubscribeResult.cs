using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Mqtt.Packets;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-11 09:11:05                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Internal
{
    /// <summary>
    /// 订阅结果
    /// </summary>
    public class SubscribeResult
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SubscribeResult()
        {

        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="topicFilter">过滤器</param>
        /// <param name="message">消息</param>
        public SubscribeResult(TopicFilter topicFilter, string message)
        {
            topicFilter.Topic = topicFilter.Topic.Trim(new char[] { '/', ' ' });
            this.Topic = topicFilter.Topic;
            this.TopicFilter = topicFilter;
            if (message.IsNotNullOrEmpty())
            {
                this.Status = false;
                this.Message = message;
            }
            else
                this.Status = true;
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="topicFilter">过滤器</param>
        public SubscribeResult(TopicFilter topicFilter) : this(topicFilter, string.Empty) { }
        #endregion

        #region 属性
        /// <summary>
        /// 订阅主题
        /// </summary>
        private string _Topic;
        /// <summary>
        /// 订阅主题
        /// </summary>
        public string Topic
        {
            get => this._Topic; 
            set
            {
                if (value == this._Topic) return;
                this._Topic = value;
                if (this.TopicFilter == null)
                    this.TopicFilter = new TopicFilter(value);
                else
                    this.TopicFilter.Topic = value;
            }
        }
        /// <summary>
        /// 过滤器
        /// </summary>
        public TopicFilter TopicFilter { get; set; }
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