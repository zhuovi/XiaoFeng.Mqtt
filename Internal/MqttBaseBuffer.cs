using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-28 16:49:18                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Internal
{
    /// <summary>
    /// 缓存基础类
    /// </summary>
    public class MqttBaseBuffer: Disposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MqttBaseBuffer()
        {

        }
        #endregion

        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// 字节转数字
        /// </summary>
        /// <param name="values">字节组</param>
        /// <returns></returns>
        public long ToValue(byte[] values)
        {
            if (values == null || values.Length == 0) return 0;
            var hexs = "";
            for (var i = 0; i < values.Length; i++)
                hexs += values[i].ToString("X");
            return Convert.ToInt64(hexs, 16);
        }
        /// <summary>
        /// 数值转字节
        /// </summary>
        /// <param name="val">数值</param>
        /// <param name="length">字节数组长度</param>
        /// <returns></returns>
        public byte[] ToBytes(long val, int length)
        {
            var bytes = new byte[length];
            for (var i = length - 1; i >= 0; i--)
            {
                bytes[length - 1 - i] = i == 0 ? (byte)(val & 0xff) : (byte)(val >> (i * 8));
            }
            return bytes;
            //return val.ToString("X").PadLeft(2 * length, '0').HexStringToBytes();
        }
        #endregion
    }
}