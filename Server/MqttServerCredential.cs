using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-14 11:44:28                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Server
{
    /// <summary>
    /// 服务端认证信息
    /// </summary>
    public class MqttServerCredential : IMqttServerCredential
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MqttServerCredential() { }
        /// <summary>
        /// 设置帐号密码
        /// </summary>
        /// <param name="userName">帐号</param>
        /// <param name="password">密码</param>
        public MqttServerCredential(string userName, string password) : this(userName, password, new List<string>())
        {
        }
        /// <summary>
        /// 设置帐号密码
        /// </summary>
        /// <param name="userName">帐号</param>
        /// <param name="password">密码</param>
        /// <param name="allowClientIp">允许IP</param>
        public MqttServerCredential(string userName, string password, string allowClientIp) : this(userName, password, new List<string>() { allowClientIp })
        {
        }
        /// <summary>
        /// 设置帐号密码
        /// </summary>
        /// <param name="userName">帐号</param>
        /// <param name="password">密码</param>
        /// <param name="allowClientIp">允许IP</param>
        public MqttServerCredential(string userName, string password, IList<string> allowClientIp)
        {
            this.UserName = userName;
            this.Password = password;
            AllowClientIp = allowClientIp;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 帐号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 允许ip
        /// </summary>
        public IList<string> AllowClientIp { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 创建服务端认证信息
        /// </summary>
        /// <param name="userName">帐号</param>
        /// <param name="password">密码</param>
        /// <param name="allowClientIp">允许IP</param>
        /// <returns></returns>
        public static IMqttServerCredential Create(string userName, string password, IList<string> allowClientIp)
        {
            return new MqttServerCredential(userName, password, allowClientIp);
        }
        /// <summary>
        /// 创建服务端认证信息
        /// </summary>
        /// <param name="userName">帐号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static IMqttServerCredential Create(string userName, string password)
        {
            return new MqttServerCredential(userName, password);
        }
        /// <summary>
        /// 创建服务端认证信息
        /// </summary>
        /// <returns></returns>
        public static IMqttServerCredential Create()
        {
            return new MqttServerCredential();
        }
        /// <summary>
        /// 创建服务端认证信息
        /// </summary>
        /// <param name="userName">帐号</param>
        /// <param name="password">密码</param>
        /// <param name="allowClientIp">允许IP</param>
        /// <returns></returns>
        public static IMqttServerCredential Create(string userName, string password, params string[] allowClientIp)
        {
            return new MqttServerCredential(userName, password, allowClientIp);
        }
        ///<inheritdoc/>
        public override string ToString()
        {
            return $"MqttServerCredential: [UserName={this.UserName}] [Password={this.Password}] [AllowClientIp=({this.AllowClientIp.Join(",")})]";
        }
        ///<inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is IMqttServerCredential credential)
            {
                return credential.UserName == this.UserName && credential.Password == this.Password && credential.AllowClientIp.Join(",") == this.AllowClientIp.Join(",");
            }
            return false;
        }
        ///<inheritdoc/>
        public static bool operator ==(MqttServerCredential credential1, MqttServerCredential credential2)
        {
            return credential1.Equals(credential2);
        }
        ///<inheritdoc/>
        public static bool operator !=(MqttServerCredential credential1, MqttServerCredential credential2)
        {
            return !credential1.Equals(credential2);
        }
        ///<inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}