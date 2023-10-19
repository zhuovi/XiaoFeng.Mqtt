using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-28 15:44:33                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Internal
{
    /// <summary>
    /// 流读取器
    /// </summary>
    public class MqttBufferReader: MqttBaseBuffer
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MqttBufferReader() { }
        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="buffer">数据</param>
        public MqttBufferReader(byte[] buffer)
        {
            this.Data = new MemoryStream();
            this.Data.Write(buffer, 0, buffer.Length);
            this.Data.Seek(0, SeekOrigin.Begin);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 内存缓存
        /// </summary>
        private MemoryStream Data { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 缓存数据长度
        /// </summary>
        public long Length => (long)this.Data?.Length;
        /// <summary>
        /// 缓存数据位置
        /// </summary>
        public long Position
        {
            get => (long)this.Data?.Position;
            set => this.Data.Position = value;
        }
        /// <summary>
        /// 是否是结束
        /// </summary>
        public bool EndOfStream => this.Data.Position == this.Data.Length;
        /// <summary>
        /// 将当前流的位置设置为指定值
        /// </summary>
        /// <param name="offset">流内的新位置。 它是相对于 origin 参数的位置，而且可正可负。</param>
        /// <param name="origin">类型 System.IO.SeekOrigin 的值，它用作查找引用点。</param>
        /// <returns>流内的新位置，通过将初始引用点和偏移量合并计算而得。</returns>
        public long Seek(long offset, SeekOrigin origin) => this.Data.Seek(offset, origin);
        /// <summary>
        /// 向前移位
        /// </summary>
        /// <param name="offset">移动位置</param>
        /// <returns></returns>
        public long Prev(ulong offset = 1) => this.Seek(-(long)offset, SeekOrigin.Current);
        /// <summary>
        /// 向后移位
        /// </summary>
        /// <param name="offset">移动位置</param>
        /// <returns></returns>
        public long Next(ulong offset = 1) => this.Seek((long)offset, SeekOrigin.Current);
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset() => this.Seek(0, SeekOrigin.Begin);
        /// <summary>
        /// 读一个字节
        /// </summary>
        /// <returns></returns>
        public int ReadByte() => this.Data.ReadByte();
        /// <summary>
        /// 读取指定长度字节
        /// </summary>
        /// <param name="length">长度 长度为0时读取剩下的所有字节</param>
        /// <returns></returns>
        public byte[] ReadBytes(long length)
        {
            if (length <= 0) length = this.Length - this.Position;
            if (length > this.Length - this.Position) length = this.Length - this.Position;
            if(length==0) return Array.Empty<byte>();
            if (!this.ValidateBufferLength((int)length)) return Array.Empty<byte>();
            var bytes = new byte[length];
            this.Data.Read(bytes, 0, (int)length);
            return bytes;
        }
        /// <summary>
        /// 读取剩余所有字节
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytes() => this.ReadBytes(0);
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public void SetBuffer(byte[] buffer)
        {
            //this.Data = new MemoryStream(buffer, true);
            this.Data = new MemoryStream();
            this.Data.Write(buffer, 0, buffer.Length);
            this.Data.Seek(0, SeekOrigin.Begin);
        }
        /// <summary>
        /// 写缓存数据
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        public void WriteBuffer(byte[] buffer)
        {
            var position = this.Data.Position;
            this.Data.Seek(0, SeekOrigin.End);
            this.Data.Write(buffer, 0, buffer.Length);
            this.Data.Position = position;
        }
        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            var length = this.ReadTwoByteInteger();
            if (length == 0) return String.Empty;
            var bytes = this.ReadBytes(length);
            return bytes.GetString();
        }
        /// <summary>
        /// 读取二进制数据
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBinaryData()
        {
            var length = this.ReadTwoByteInteger();
            if (length == 0) return Array.Empty<byte>();
            if (!ValidateBufferLength(length)) return Array.Empty<byte>();
            return this.ReadBytes(length);
        }
        /// <summary>
        /// 读取2个字节整型数字
        /// </summary>
        /// <returns></returns>
        public ushort ReadTwoByteInteger()
        {
            if (!ValidateBufferLength(2)) return 0;
            var msb = this.ReadByte();
            var lsb = this.ReadByte();
            return (ushort)(msb << 8 | lsb);
        }
        /// <summary>
        /// 读取4个字节整型数字
        /// </summary>
        /// <returns></returns>
        public uint ReadFourByteInteger()
        {
            if (!ValidateBufferLength(4)) return 0;
            var byte0 = this.ReadByte();
            var byte1 = this.ReadByte();
            var byte2 = this.ReadByte();
            var byte3 = this.ReadByte();

            return (uint)((byte0 << 24) | (byte1 << 16) | (byte2 << 8) | byte3);
        }
        /// <summary>
        /// 读取可变小字节整型数字
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">异常</exception>
        public uint ReadVariableByteInteger()
        {
            return this.ReadVariableByteInteger(out var _);
        }
        /// <summary>
        /// 读取可变小字节整型数字
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">异常</exception>
        public uint ReadVariableByteInteger(out int size)
        {
            var multiplier = 1;
            var value = 0U;
            byte encodedByte;
            size = 0;
            do
            {
                encodedByte = (byte)this.ReadByte();
                value += (uint)((encodedByte & 127) * multiplier);
                if (multiplier > (128 * 128 * 128))
                {
                    throw new MqttException("Malformed Remaining Length.");
                }
                multiplier *= 128;
                size++;
            } while ((encodedByte & 128) != 0 && size <= 3);

            return value;
        }
        /// <summary>
        /// 验证流长度
        /// </summary>
        /// <param name="length">长度</param>
        /// <exception cref="Exception">异常</exception>
        bool ValidateBufferLength(int length)
        {
            if (this.Length == 0) throw new Exception("当前缓存流没有数据.");
            var newPosition = this.Position + length;

            if (this.Length < newPosition)
            {
                LogHelper.Error(new MqttException($"需要至少 {length} 个字节，但当前缓存流只有 {this.Length - this.Position} 字节可读."));
                LogHelper.Debug(this.Data.ToArray().Join(" "));
                return false;
            }
            return true;
        }
        #endregion
    }
}