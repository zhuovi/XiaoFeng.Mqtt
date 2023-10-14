using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-06-16 16:15:19                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt
{
    /*
     *  ---------------------------------------------------------------------------------------------------------
     *  值	        返回码响应	                                描述
     *  ---------------------------------------------------------------------------------------------------------
     *  0	        0x00连接已接受	                            连接已被服务端接受
     *  1	        0x01连接已拒绝，不支持的协议版本	        服务端不支持客户端请求的MQTT协议级别
     *  2	        0x02连接已拒绝，不合格的客户端标识符	    客户端标识符是正确的UTF-8编码，但服务端不允许使用
     *  3	        0x03连接已拒绝，服务端不可用	            网络连接已建立，但MQTT服务不可用
     *  4	        0x04连接已拒绝，无效的用户名或密码	        用户名或密码的数据格式无效
     *  5	        0x05连接已拒绝，未授权	                    客户端未被授权连接到此服务器
     *  6-255		保留
     */
    /// <summary>
    /// 连接返回码
    /// </summary>
    public enum ConnectReturnCode:byte
    {
        /// <summary>
        /// 连接已接受
        /// </summary>
        /// <remarks>连接已被服务端接受</remarks>
        ACCEPTED = 0x00,
        /// <summary>
        /// 连接已拒绝，不支持的协议版本
        /// </summary>
        /// <remarks>服务端不支持客户端请求的MQTT协议级别</remarks>
        REFUSED_UNACCEPTABLE_PROTOCOL_VERSION = 0x01,
        /// <summary>
        /// 连接已拒绝，不合格的客户端标识符
        /// </summary>
        /// <remarks>客户端标识符是正确的UTF-8编码，但服务端不允许使用</remarks>
        REFUSED_IDENTIFIER_REJECTED = 0x02,
        /// <summary>
        /// 连接已拒绝，服务端不可用
        /// </summary>
        /// <remarks>网络连接已建立，但MQTT服务不可用</remarks>
        REFUSED_SERVER_UNAVAILABLE = 0x03,
        /// <summary>
        /// 连接已拒绝，无效的用户名或密码
        /// </summary>
        /// <remarks>用户名或密码的数据格式无效</remarks>
        REFUSED_BAD_USERNAME_OR_PASSWORD = 0x04,
        /// <summary>
        /// 连接已拒绝，未授权
        /// </summary>
        /// <remarks>客户端未被授权连接到此服务器</remarks>
        REFUSED_NOT_AUTHORIZED = 0x05
    }
}