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
*  Create Time : 2023-10-12 20:46:30                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt
{
    /// <summary>
    /// 返回结果包
    /// </summary>
    public class ResultPacket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ResultPacket() { this.ResultType = ResultType.Success; }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="mqttPacket">包</param>
        /// <param name="resultType">结果类型</param>
        /// <param name="message">消息</param>
        public ResultPacket(MqttPacket mqttPacket, ResultType resultType, string message)
        {
            ResultType = resultType;
            Message = message;
            MqttPacket = mqttPacket;
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="mqttPacket">包</param>
        /// <param name="resultType">结果类型</param>
        /// <param name="reasonCode">原因码</param>
        /// <param name="message">消息</param>
        public ResultPacket(MqttPacket mqttPacket, ResultType resultType, ReasonCode reasonCode, string message)
        {
            ResultType = resultType;
            Message = message;
            ReasonCode = reasonCode;
            MqttPacket = mqttPacket;
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="resultType">结果类型</param>
        /// <param name="message">消息</param>
        public ResultPacket(ResultType resultType, string message) : this(null, resultType, message) { }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="mqttPacket">包</param>
        /// <param name="message">消息</param>
        public ResultPacket(MqttPacket mqttPacket, string message) : this(mqttPacket, ResultType.Error, message) { }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="mqttPacket">包</param>
        public ResultPacket(MqttPacket mqttPacket) : this(mqttPacket, ResultType.Success, "") { }
        #endregion

        #region 属性
        /// <summary>
        /// 结果类型
        /// </summary>
        public ResultType ResultType { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 包
        /// </summary>
        public MqttPacket MqttPacket { get; set; }
        /// <summary>
        /// 原因码
        /// </summary>
        public ReasonCode ReasonCode { get; set; }
        #endregion

        #region 方法

        #endregion
    }
    /// <summary>
    /// 包类型
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,
        /// <summary>
        /// 失败
        /// </summary>
        Error = 1
    }
}