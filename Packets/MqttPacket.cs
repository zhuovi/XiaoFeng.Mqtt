using System;
using System.IO;
using System.Linq;
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
            this.Reader.Callback = m =>
            {
                this.SetError(ReasonCode.MALFORMED_PACKET, m);
            };
            this.UnPacket();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 数据总长
        /// </summary>
        private long _TotalLength = 0;
        /// <summary>
        /// 数据总长
        /// </summary>
        public long TotalLength { get { return _TotalLength; } }
        /// <summary>
        /// 当前长度
        /// </summary>
        public long Length => this.Reader.Length - this.Reader.Position;
        /// <summary>
        /// 是否有分片
        /// </summary>
        private bool _IsSharding = false;
        /// <summary>
        /// 是否有分片
        /// </summary>
        public bool IsSharding => this._IsSharding;
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
        /// <summary>
        /// 解包状态
        /// </summary>
        public PacketStatus PacketStatus { get; set; } = PacketStatus.Success;
        /// <summary>
        /// 错误码
        /// </summary>
        public ReasonCode ErrorCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="reasonCode">错误码</param>
        /// <param name="errorMessage">错误消息</param>
        public void SetError(ReasonCode reasonCode, string errorMessage = "")
        {
            this.ErrorCode = reasonCode;
            this.ErrorMessage = errorMessage.IsNullOrEmpty() ? reasonCode.GetDescription() : errorMessage;
            this.PacketStatus = PacketStatus.Error;
        }
        /// <summary>
        /// 读取剩下字节
        /// </summary>
        /// <returns></returns>
        public byte[] ReadRemainingBytes() => this.Reader.ReadBytes();
        /// <summary>
        /// 获取字节数组
        /// </summary>
        /// <returns></returns>
        public virtual byte[] ToArray()
        {
            var ms = new MqttBufferWriter();

            this.PacketType = GetPacketType();
            
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
            this.Reader.Position = 0;
            var flag = this.Reader.ReadByte();

            this.PacketType = (PacketType)(flag >> 4);
            if ((int)this.PacketType < 0 || (int)this.PacketType > 15)
            {
                this.SetError(ReasonCode.MALFORMED_PACKET, "无效报文.");
                this.PacketStatus = PacketStatus.Error;
                return;
            }
            this._FixedFlags = flag & 0x0F;
            this.SetFlags(flag);
            var length = this.Reader.ReadVariableByteInteger(out var size);
            if (length == 0) return;
            this._TotalLength = length;
            if (length > this.Reader.Length - this.Reader.Position)
            {   
                this._IsSharding = true;
                return;
            }
            var reader = new MqttBufferReader(this.Reader.ReadBytes(length));
            if (!this.ReadBuffer(reader))
            {
                this.PacketStatus = PacketStatus.Error;
            }
        }
        /// <summary>
        /// 写buffer
        /// </summary>
        /// <param name="buffer">数据</param>
        public void WriteBuffer(byte[] buffer)
        {
            var position = this.Reader.Position;
            this.Reader.WriteBuffer(buffer);
            this.Reader.Position = position;
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
        /// <summary>
        /// 获取包类型
        /// </summary>
        /// <returns></returns>
        PacketType GetPacketType()
        {
            if (this is ConnectPacket) return PacketType.CONNECT;
            if (this is ConnectActPacket) return PacketType.CONNACK;
            if(this is DisconnectPacket)return PacketType.DISCONNECT;
            if (this is AuthPacket) return PacketType.AUTH;
            if (this is PingReqPacket) return PacketType.PINGREQ;
            if (this is PingRespPacket) return PacketType.PINGRESP;
            if (this is PubAckPacket) return PacketType.PUBACK;
            if (this is PubCompPacket) return PacketType.PUBCOMP;
            if(this is PubRecPacket) return PacketType.PUBREC;
            if (this is PubRelPacket) return PacketType.PUBREL;
            if (this is PublishPacket) return PacketType.PUBLISH;
            if (this is SubscribePacket) return PacketType.SUBSCRIBE;
            if (this is SubAckPacket) return PacketType.SUBACK;
            if(this is UnsubscribePacket) return PacketType.UNSUBSCRIBE;
            if (this is UnsubAckPacket) return PacketType.UNSUBACK;
            return 0;
        }
        #endregion
    }
}