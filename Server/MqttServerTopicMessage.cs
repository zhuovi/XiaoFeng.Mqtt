using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using XiaoFeng.Mqtt.Packets;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-14 12:31:18                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Server
{
    /// <summary>
    /// MQTT服务端主题消息
    /// </summary>
    public class MqttServerTopicMessage : IMqttServerTopicMessage
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MqttServerTopicMessage()
        {

        }
        /// <summary>
        /// 设置主题消息
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="packet">消息</param>
        /// <param name="expiredTime">过期时间</param>
        public MqttServerTopicMessage(string topic, PublishPacket packet, int expiredTime)
        {
            Topic = topic;
            PublishPacket = packet;
            this.ExpiredTime = expiredTime;
        }
        /// <summary>
        /// 设置主题消息
        /// </summary>
        /// <param name="packet">消息</param>
        public MqttServerTopicMessage(PublishPacket packet) : this(packet.Topic, packet, DateTime.Now.ToTimeStamp() + 600) { }
        #endregion

        #region 属性
        /// <summary>
        /// 主题
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// 消息包
        /// </summary>
        public PublishPacket PublishPacket { get; set; }
        /// <summary>
        /// 分发次数
        /// </summary>
        private int _DistributeCount = 0;
        /// <summary>
        /// 分发次数
        /// </summary>
        public int DistributeCount => this._DistributeCount;
        /// <summary>
        /// 过期时间
        /// </summary>
        public int ExpiredTime { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 添加分发次数
        /// </summary>
        public void AddDistributeCount()
        {
            Interlocked.Increment(ref this._DistributeCount);
        }
        /// <summary>
        /// 创建主题消息
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="publishPacket">消息</param>
        /// <param name="expiredTime">过期时间</param>
        /// <returns></returns>
        public static IMqttServerTopicMessage Create(string topic, PublishPacket publishPacket, int expiredTime)
        {
            return new MqttServerTopicMessage(topic, publishPacket, expiredTime);
        }
        public static IMqttServerTopicMessage Create(PublishPacket publishPacket, int expiredTime) => Create(publishPacket.Topic, publishPacket, expiredTime);
        /// <summary>
        /// 主题
        /// </summary>
        /// <param name="publishPacket">消息</param>
        /// <returns></returns>
        public static IMqttServerTopicMessage Create(PublishPacket publishPacket) => Create(publishPacket, DateTime.Now.ToTimeStamp() + 600);
        ///<inheritdoc/>
        public override string ToString()
        {
            return $"MqttServerTopicMessage: [Topic={this.Topic}] [DistributeCount={this.DistributeCount}] [ExpiredTime={this.ExpiredTime}] [Publish=({this.PublishPacket})]";
        }
        #endregion
    }
}