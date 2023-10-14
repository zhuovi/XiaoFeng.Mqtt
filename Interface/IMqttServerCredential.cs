using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-14 11:54:10                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt
{
    /// <summary>
    /// 服务端认证信息接口
    /// </summary>
    public interface IMqttServerCredential
    {
        #region 属性
        /// <summary>
        /// 帐号
        /// </summary>
        string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        string Password { get; set; }
        /// <summary>
        /// 允许ip
        /// </summary>
        IList<string> AllowClientIp { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}