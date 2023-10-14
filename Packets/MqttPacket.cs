using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XiaoFeng.Mqtt.Internal;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-06-16 15:24:29                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Packets
{
    /// <summary>
    /// MQTT（Message Queue Telemetry Transport）,遥测传输协议
    /// </summary>
    /// <remarks>报文包</remarks>
    public abstract class MqttPacket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MqttPacket()
        {
            this.ProtocolVersion = MqttProtocolVersion.V500;
        }
        /// <summary>
        /// 设置协议版本
        /// </summary>
        /// <param name="protocolVersion">协议版本</param>
        public MqttPacket(MqttProtocolVersion protocolVersion)
        {
            this.ProtocolVersion = protocolVersion;
        }
        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="buffer">字节组</param>
        public MqttPacket(byte[] buffer) : this(buffer, MqttProtocolVersion.V500) { }
        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="buffer">字节组</param>
        /// <param name="protocolVersion">协议版本</param>
        public MqttPacket(byte[] buffer, MqttProtocolVersion protocolVersion)
        {
            if (buffer == null || buffer.Length == 0) return;
            this.ProtocolVersion = protocolVersion;
            this._PacketSize = buffer.Length;
            this.Reader = new MqttBufferReader(buffer);
            this.UnPacket();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 缓存读取器
        /// </summary>
        private MqttBufferReader Reader { get; set; }
        /// <summary>
        /// 报文类型
        /// </summary>
        public PacketType PacketType { get; set; }
        /// <summary>
        /// 协议级别
        /// </summary>
        /// <remarks>客户端用8位的无符号值表示协议的修订版本。
        /// 对于3.1.1版协议，协议级别字段的值是4(0x04)。
        /// 如果发现不支持的协议级别，服务端必须给发送一个返回码为0x01（不支持的协议级别）的CONNACK报文响应CONNECT报文，然后断开客户端的连接。
        /// </remarks>
        public MqttProtocolVersion ProtocolVersion { get; set; }
        /// <summary>
        /// 固定头标识位
        /// </summary>
        private int _FixedFlags;
        /// <summary>
        /// 固定头标识位
        /// </summary>
        public int FixedFlags => this._FixedFlags;
        /// <summary>
        /// 包长
        /// </summary>
        private int _PacketSize = 0;
        /// <summary>
        /// 包长
        /// </summary>
        public int PacketSize => this._PacketSize;
        #endregion

        #region 方法
        /// <summary>
        /// 获取字节数组
        /// </summary>
        /// <returns></returns>
        public virtual byte[] ToArray()
        {
            var ms = new MqttBufferWriter();
            ms.WriteByte(this.GetFlags());
            var Payload = new MqttBufferWriter();
            if (!this.WriteBuffer(Payload)) return Array.Empty<byte>();
            ms.WriteVariableByteInteger((uint)Payload.Length);
            ms.WriteBytes(Payload.ToArray());
            var bytes = ms.ToArray();
            this._PacketSize = bytes.Length;
            return bytes;
        }
        /// <summary>
        /// 解包
        /// </summary>
        public virtual void UnPacket()
        {
            var flag = this.Reader.ReadByte();

            this.PacketType = (PacketType)(flag >> 4);
            this._FixedFlags = flag & 0x0F;
            this.SetFlags(flag);
            var length = this.Reader.ReadVariableByteInteger();
            if (length == 0) return;
            var reader = new MqttBufferReader(this.Reader.ReadBytes(length));
            if (this.ReadBuffer(reader)) return;
        }
        /// <summary>
        /// 写流
        /// </summary>
        /// <param name="writer">写流器</param>
        /// <returns></returns>
        public virtual bool WriteBuffer(MqttBufferWriter writer) => true;
        /// <summary>
        /// 读流
        /// </summary>
        /// <param name="reader">读流器</param>
        /// <returns></returns>
        public virtual bool ReadBuffer(MqttBufferReader reader) => true;
        /// <summary>
        /// 设置flags
        /// </summary>
        /// <param name="flags">标识</param>
        public virtual void SetFlags(int flags) { }
        /// <summary>
        /// 获取标识
        /// </summary>
        /// <returns></returns>
        public virtual byte GetFlags(int flags = 0)
        {
            var fixedHeader = (byte)PacketType << 4;
            if (flags > 0)
                fixedHeader |= flags;
            return (byte)fixedHeader;
        }
        #endregion
    }
}