using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Collections;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-09 15:54:41                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Packets
{
    /// <summary>
    /// 订阅选项
    /// </summary>
    public class TopicFilter
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public TopicFilter()
        {

        }
        /// <summary>
        /// 设置订阅选项
        /// </summary>
        /// <param name="topic">订阅主题</param>
        public TopicFilter(string topic)
        {
            if (topic.IsNullOrEmpty()) return;
            topic = topic.Trim(' ').TrimEnd('/');
            if (topic.StartsWith("TopicFilter:", StringComparison.OrdinalIgnoreCase))
            {
                topic = topic.RemovePattern(@"^TopicFilter: ").ReplacePattern(@"\] \[", "&").Trim(new char[] { '[', ']' });
                ParameterCollection p = new ParameterCollection(topic);
                this.Topic = p.Get("topic");
                this.NoLocal = p.Get("NoLocal").EqualsIgnoreCase("true");
                this.RetainAsPublished = p.Get("RetainAsPublished").EqualsIgnoreCase("true");
                this.QualityOfServiceLevel = p.Get("QualityOfServiceLevel").ToEnum<QualityOfServiceLevel>();
                this.RetainHandling = p.Get("RetainHandling").ToEnum<RetainHandling>();
            }
            else
            {
                this.Topic = topic;
            }
        }
        /// <summary>
        /// 设置订阅选项
        /// </summary>
        /// <param name="topic">订阅主题</param>
        /// <param name="qualityOfServiceLevel">质量等级</param>
        public TopicFilter(string topic, QualityOfServiceLevel qualityOfServiceLevel)
        {
            this.Topic = topic;
            this.QualityOfServiceLevel = qualityOfServiceLevel;
        }
        /// <summary>
        /// 设置订阅选项
        /// </summary>
        /// <param name="topic">订阅主题</param>
        /// <param name="noLocal">非本地项</param>
        /// <param name="retainAsPublished">是否保留为已发布</param>
        /// <param name="retainHandling">已保存的消息发送类型</param>
        /// <param name="qualityOfServiceLevel">质量等级</param>
        public TopicFilter(string topic, bool noLocal, bool retainAsPublished, RetainHandling retainHandling, QualityOfServiceLevel qualityOfServiceLevel)
        {
            this.Topic = topic;
            this.NoLocal = noLocal;
            this.RetainHandling = retainHandling;
            this.QualityOfServiceLevel = qualityOfServiceLevel;
            this.RetainAsPublished = retainAsPublished;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 非本地项
        /// </summary>
        /// <remarks>
        /// 如果为 1，表示 Application Message 不能转发到 ClientID 与发布来源相同的网络连接上[MQTT-3.8.3-3]。即不要把自身的发布，再发给自身。对于共享订阅设置 No Local选项为 1 认为是 Protocol Error[MQTT-3.8.3-4]。
        /// </remarks>
        public bool NoLocal { get; set; }
        /// <summary>
        /// 质量等级
        /// </summary>
        public QualityOfServiceLevel QualityOfServiceLevel { get; set; }
        /// <summary>
        /// 是否保留为已发布
        /// </summary>
        public bool RetainAsPublished { get; set; }
        /// <summary>
        /// 已保存的消息发送类型
        /// </summary>
        public RetainHandling RetainHandling { get; set; }
        /// <summary>
        /// 订阅主题
        /// </summary>
        public string Topic { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is TopicFilter topic)
            {
                return topic.Topic == this.Topic && topic.NoLocal == this.NoLocal && topic.QualityOfServiceLevel == this.QualityOfServiceLevel && topic.RetainAsPublished == this.RetainAsPublished && topic.RetainHandling == this.RetainHandling;
            }
            return false;
        }
        /// <summary>
        /// 两类型相等
        /// </summary>
        /// <param name="a">第一个对象</param>
        /// <param name="b">第二个对象</param>
        /// <returns></returns>
        public static bool operator ==(TopicFilter a, TopicFilter b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            if (a is null || b is null)
            {
                return false;
            }
            return a.Equals(b);
        }
        /// <summary>
        /// 两类型不相等
        /// </summary>
        /// <param name="a">第一个对象</param>
        /// <param name="b">第二个对象</param>
        /// <returns></returns>
        public static bool operator !=(TopicFilter a, TopicFilter b)
        {
            return !(a == b);
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="v">值</param>
        public static explicit operator string(TopicFilter v)
        {
            return v.ToString();
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator TopicFilter(string v)
        {
            if (v.IsNullOrEmpty()) return new TopicFilter();
            return new TopicFilter(v);
        }
        ///<inheritdoc/>
        public override string ToString()
        {
            return
                $"TopicFilter: [Topic={this.Topic}] [QualityOfServiceLevel={this.QualityOfServiceLevel}] [NoLocal={this.NoLocal}] [RetainAsPublished={this.RetainAsPublished}] [RetainHandling={this.RetainHandling}]";
        }
        /// <summary>
        /// 重写HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return
#if NETSTANDARD2_1
                HashCode.Combine(NoLocal, QualityOfServiceLevel, RetainAsPublished, RetainHandling, Topic)
#else
                (NoLocal ? 1 : 0) << 1 | ((int)QualityOfServiceLevel) << 7 | (RetainAsPublished ? 1 : 0) << 12 | ((int)RetainHandling) << 18 | Topic.GetHashCode() << 24
#endif
                ;
        }
#endregion
    }
}