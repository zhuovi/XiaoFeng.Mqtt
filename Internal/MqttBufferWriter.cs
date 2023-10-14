using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XiaoFeng.Mqtt.Protocol;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-28 10:28:10                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Internal
{
    /// <summary>
    /// 流写入器
    /// </summary>
    public class MqttBufferWriter: MqttBaseBuffer
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MqttBufferWriter()
        {
            this.Data = new MemoryStream();
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="buffer">数据</param>
        public MqttBufferWriter(MqttBufferWriter buffer)
        {
            this.Data = new MemoryStream(buffer.ToArray());
        }
        #endregion

        #region 属性
        /// <summary>
        /// 数值最大长度
        /// </summary>
        const uint VariableByteIntegerMaxValue = 268435455;
        /// <summary>
        /// 字符串最大长度
        /// </summary>
        const int StringMaxLength = 65535;
        /// <summary>
        /// 内存流
        /// </summary>
        private MemoryStream Data { get; set; }
        /// <summary>
        /// 流的长度
        /// </summary>
        public long Length => (long)this.Data?.Length;
        /// <summary>
        /// 缓存数据位置
        /// </summary>
        public long Position => (long)this.Data?.Position;
        #endregion

        #region 方法
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset() => this.Data = new MemoryStream();
        /// <summary>
        /// 创建此流的无符号字节的数组。
        /// </summary>
        public byte[] GetBuffer() => this.Data == null ? Array.Empty<byte>() : this.Data.GetBuffer();
        /// <summary>
        /// 将流内容写入字节数组，而与 Position 属性无关。
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray() => this.Data == null ? Array.Empty<byte>() : this.Data.ToArray();
        /// <summary>
        /// 写字节
        /// </summary>
        /// <param name="byte">字节</param>
        public void WriteByte(byte @byte) => this.Data.WriteByte(@byte);
        /// <summary>
        /// 写字符组
        /// </summary>
        /// <param name="bytes">字节组</param>
        public void WriteBytes(byte[] bytes) => this.Data.Write(bytes, 0, bytes.Length);
        /// <summary>
        /// 写属性
        /// </summary>
        /// <param name="id">属性ID</param>
        /// <param name="value">属性值</param>
        /// <param name="length">占用字节数</param>
        public void WriteProperty(MqttPropertiesId id, long value, int length)
        {
            if (value == 0) return;
            this.WriteByte((byte)id);
            this.WriteBytes(this.ToBytes(value, length));
        }
        /// <summary>
        /// 写属性
        /// </summary>
        /// <param name="id">属性ID</param>
        /// <param name="value">属性值</param>
        public void WriteVariableProperty(MqttPropertiesId id, uint value)
        {
            if (value == 0) return;
            this.WriteByte((byte)id);
            this.WriteVariableByteInteger(value);
        }
        /// <summary>
        /// 写属性
        /// </summary>
        /// <param name="id">属性ID</param>
        /// <param name="value">属性值</param>
        public void WriteProperty(MqttPropertiesId id, string value)
        {
            if (value.IsNullOrEmpty()) return;
            this.WriteByte((byte)id);
            this.WriteString(value);
        }
        /// <summary>
        /// 写属性
        /// </summary>
        /// <param name="id">属性ID</param>
        /// <param name="value">属性值</param>
        public void WriteProperty(MqttPropertiesId id, bool value)
        {
            this.WriteByte((byte)id);
            this.WriteByte(value ? (byte)0x1 : (byte)0x0);
        }
        /// <summary>
        /// 写属性
        /// </summary>
        /// <param name="id">属性ID</param>
        /// <param name="value">属性值</param>
        public void WriteProperty(MqttPropertiesId id, byte value)
        {
            this.WriteByte((byte)id);
            this.WriteByte(value);
        }
        /// <summary>
        /// 写属性
        /// </summary>
        /// <param name="id">属性ID</param>
        /// <param name="value">属性值</param>
        public void WriteProperty(MqttPropertiesId id, byte[] value)
        {
            if (value == null || value.Length == 0) return;
            this.WriteByte((byte)id);
            this.WriteBytes(value);
        }
        /// <summary>
        /// 写用户属性
        /// </summary>
        /// <param name="userProperties">用户属性</param>
        public void WriteUserProperties(List<MqttUserProperty> userProperties)
        {
            if (userProperties == null || userProperties.Count == 0) return;
            userProperties.Each(p =>
            {
                this.WriteByte((byte)MqttPropertiesId.UserProperty);
                this.WriteString(p.Name);
                this.WriteString(p.Value);
            });
        }
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="value">字符串</param>
        public void WriteString(string value)
        {
            if (value.IsNullOrEmpty())
            {
                this.WriteBytes(new byte[] { 0, 0 });
            }
            else
            {
                var bytes = value.GetBytes(Encoding.UTF8);
                if (bytes.Length > StringMaxLength) throw new Exception($"超过最大字符串长度为{StringMaxLength},当前字符串的长度为 {bytes.Length}.");
                this.WriteBytes(ToBytes(bytes.Length, 2));
                this.WriteBytes(bytes);
            }
        }
        /// <summary>
        /// 写数字
        /// </summary>
        /// <param name="bytes">字节组</param>
        public void WriteBinaryData(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                this.WriteBytes(new byte[] { 0, 0 });
            else
            {
                this.WriteBytes(ToBytes(bytes.Length, 2));
                this.WriteBytes(bytes);
            }
        }
        /// <summary>
        /// 转换几位数字
        /// </summary>
        /// <param name="value">数字</param>
        /// <param name="length">占字节位数</param>
        public void WriteValue(long value, int length)
        {
            this.WriteBytes(ToBytes(value, length));
        }
        /// <summary>
        /// 写两个字节的数字
        /// </summary>
        /// <param name="value">数字</param>
        public void WriteTwoByteInteger(int value)
        {
            this.WriteBytes(ToBytes(value, 2));
        }
        /// <summary>
        /// 写四个字节的数字
        /// </summary>
        /// <param name="value">数字</param>
        public void WriteFourByteInteger(int value)
        {
            this.WriteBytes(ToBytes(value, 4));
        }
        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="source">源缓存</param>
        public void Write(MqttBufferWriter source)
        {
            if (source == null || source.Length == 0) return;
            var bytes = source.ToArray();
            var length = this.GetVariableByteIntegerSize(bytes.Length);
            this.WriteBytes(this.ToBytes(bytes.Length, length));
            this.WriteBytes(bytes);
        }
        /// <summary>
        /// 写可变字节整数
        /// </summary>
        /// <param name="value">值</param>
        /// <exception cref="Exception">异常</exception>
        public void WriteVariableByteInteger(uint value)
        {
            if (value < 128)
            {
                this.WriteByte((byte)value);
                return;
            }
            if (value > VariableByteIntegerMaxValue) throw new MqttException($"指定的值( {value} )对于可变字节整数来说太大.");

            var x = value;
            do
            {
                var encodedByte = x % 128;
                x /= 128;
                if (x > 0)
                {
                    encodedByte |= 128;
                }
                WriteByte((byte)encodedByte);
            } while (x > 0);
        }
        /// <summary>
        /// 生成固定标头
        /// </summary>
        /// <param name="packetType">报文类型</param>
        /// <param name="flags">标识</param>
        /// <returns></returns>
        public byte BuildFixedHeader(PacketType packetType, int flags = 0)
        {
            var fixedHeader = (int)packetType << 4;
            if (flags == 0)
            {
                flags = packetType == PacketType.PUBREL || packetType == PacketType.SUBSCRIBE || packetType == PacketType.UNSUBSCRIBE ? 2 : 0;
            }
            fixedHeader |= flags;
            return (byte)fixedHeader;
        }
        /// <summary>
        /// 写固定标头
        /// </summary>
        /// <param name="packetType">报文类型</param>
        public virtual void WriteFixedHeader(PacketType packetType, int flags = 0)
        {
            var fixedHeader = this.BuildFixedHeader(packetType, flags);
            this.WriteByte(fixedHeader);
        }        
        /// <summary>
        /// 获取可变字节占用字节数
        /// </summary>
        /// <param name="length">可变字节长度</param>
        /// <returns></returns>
        /// <exception cref="Exception">超过指定长度则抛出异常</exception>
        public int GetVariableByteIntegerSize(int length)
        {
            if (length < 0x7F) return 1;
            if (length < 0xFF7F) return 2;
            if (length < 0xFFFF7F) return 3;
            if (length > VariableByteIntegerMaxValue) throw new Exception($"指定的值( {length} )对于可变字节整数来说太大.");
            return 4;
        }
        #endregion

        #region 释放
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">标识</param>
        protected override void Dispose(bool disposing)
        {
            this.Data?.Dispose();
            this.Data = null;
            base.Dispose(disposing);
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~MqttBufferWriter()
        {
            this.Dispose(false);
        }
        #endregion
    }
}