using System.Threading.Tasks;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-12 14:39:49                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Server
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class PrototypeHelper
    {
        #region 属性
        /// <summary>
        /// 客户端数据 key
        /// </summary>
        public const string ClientDataKey = "MqttClientData";
        #endregion

        #region 方法
        /// <summary>
        /// 初始化客户端数据
        /// </summary>
        /// <param name="client">客户端</param>
        public static void InitClientData(this ISocketClient client)
        {
            client.AddData(ClientDataKey, new MqttClientData());
        }
        /// <summary>
        /// 获取客户端数据
        /// </summary>
        /// <param name="client">客户端</param>
        /// <returns></returns>
        public static MqttClientData GetClientData(this ISocketClient client)
        {
            if (client == null) return null;
            if (client.TryGetData(ClientDataKey, out var data))
            {
                return (MqttClientData)data;
            }
            else
            {
                var _ = new MqttClientData();
                client.AddData(ClientDataKey, _);
                return _;
            }
        }
        #endregion
    }
}