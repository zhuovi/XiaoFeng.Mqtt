using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-28 08:55:39                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt
{
    /// <summary>
    /// 用户自定义属性
    /// </summary>
    public sealed class MqttUserProperty
    {
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <exception cref="ArgumentNullException">参数空异常</exception>
        public MqttUserProperty(string name, string value)
        {
            if (name.IsNullOrEmpty()) throw new ArgumentNullException(nameof(name));
            if (value.IsNullOrEmpty()) throw new ArgumentNullException(nameof(value));

        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; }
        /// <summary>
        /// 两个对象是否相等
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is MqttUserProperty)) return false;
            return Equals(obj as MqttUserProperty);
        }
        /// <summary>
        /// 两个对象是否相等
        /// </summary>
        /// <param name="other">其它对象</param>
        /// <returns></returns>
        public bool Equals(MqttUserProperty other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name.EqualsIgnoreCase(other.Name) && Value.EqualsIgnoreCase(other.Value);
        }
        /// <summary>
        /// 返回哈希代码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Value.GetHashCode();
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name} = {Value}";
        }
    }
}