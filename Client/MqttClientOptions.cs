using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using XiaoFeng.Mqtt.Packets;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-08 09:03:23                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Client
{
    /// <summary>
    /// Mqtt 客户端连接配置
    /// </summary>
    public class MqttClientOptions : ConnectPacket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MqttClientOptions()
        {
            base.ClientId = "EELFMQTT" + RandomHelper.GetRandomInt(100000, 1000000);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否启用SSL
        /// </summary>
        public bool UseTls { get; set; }
        /// <summary>
        /// SSL协议版本
        /// </summary>
        public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls12;
        /// <summary>
        /// 客户端证书
        /// </summary>
        public X509CertificateCollection ClientCertificates { get; set; }
        /// <summary>
        /// 通讯类型
        /// </summary>
        public SocketTypes SocketType { get; set; }
        /// <summary>
        /// 是否允许数据包分片
        /// </summary>
        public bool AllowPacketFragmentation { get; set; } = true;
        /// <summary>
        /// 连接超时时间
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
        /// <summary>
        /// 读超时时间 单位秒
        /// </summary>
        public int ReadTimeout { get; set; } = 10;
        /// <summary>
        /// 写超时时间 单位秒
        /// </summary>
        public int WriteTimeout { get; set; } = 10;
        /// <summary>
        /// 写入缓存大小
        /// </summary>
        public int WriteBufferSize { get; set; } = 8192;
        /// <summary>
        /// 读缓存最大值
        /// </summary>
        public int ReadeBufferSize { get; set; } = 8192;
        /// <summary>
        /// 断开是否自动连接
        /// </summary>
        public bool IsAutoConnect { get; set; } = false;
        /// <summary>
        /// 自动连接周期 单位秒
        /// </summary>
        private int _ReConnectPeriod = 3;
        /// <summary>
        /// 自动连接周期 单位秒
        /// </summary>
        public int ReConnectPeriod
        {
            get
            {
                if (this._ReConnectPeriod <= 0)
                    this._ReConnectPeriod = 1;
                return this._ReConnectPeriod;
            }
            set => this._ReConnectPeriod = value <= 0 ? 1 : value;
        }
        #endregion

        #region 方法

        #endregion
    }
}