using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Mqtt.Packets;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2025) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2025-06-10 10:02:21                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Server
{
    /// <summary>
    /// 分发模型
    /// </summary>
    public class DistributeData
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        public DistributeData()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 分消息客户端
        /// </summary>
        public ISocketClient Client { get; set; }
        /// <summary>
        /// 分发主题
        /// </summary>
        public TopicFilter Topic { get; set; }
        /// <summary>
        /// 分发状态
        /// </summary>
        public bool Status { get; set; }
        #endregion

        #region 方法
        #region 析构器
        /// <summary>
        /// 析构器
        /// </summary>
        ~DistributeData()
        {

        }
        #endregion
        #endregion
    }
}