using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-11 08:56:07                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Mqtt.Internal
{
    /// <summary>
    /// 帮助类
    /// </summary>
    public static class MqttHelper
    {
        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// 验证主题名称的合法性 true是合法
        /// </summary>
        /// <param name="topic">主题名称</param>
        /// <returns></returns>
        public static bool IsValidTopicName(string topic)
        {
            return topic.Length > 0 && topic.Length <= 65535;           
        }
        /// <summary>
        /// 验证过过滤器的合法性 true是合法
        /// </summary>
        /// <param name="topicFilter">过滤器</param>
        /// <returns></returns>
        /// <remarks>
        /// <para>过滤器合法格式</para>
        /// <para><term>1</term> # 匹配所有主题</para>
        /// <para><term>2</term> + 匹配一个层级主题</para>
        /// <para><term>3</term> a/b/# 匹配前边有a/b/下的所有主题</para>
        /// <para><term>4</term> /a/b 和 #/a/b 是一样的</para>
        /// </remarks>
        public static bool IsValidTopicFilter(string topicFilter)
        {
            if (!IsValidTopicName(topicFilter)) return false;
            if (topicFilter.Contains("#") && topicFilter.IsMatch(@"(#\/|[a-z0-9]#|#[a-z0-9])")) return false;
            if (topicFilter.Contains("+") && topicFilter.IsMatch(@"([a-z0-9]\+|\+\[a-z0-9])")) return false;
            return true;
        }
        /// <summary>
        /// 是否是共享主题
        /// </summary>
        /// <param name="topicFilter">过滤器</param>
        /// <returns></returns>
        public static bool IsSharedTopicFilter(string topicFilter)
        {
            if (!IsValidTopicFilter(topicFilter)) return false;
            if(!topicFilter.StartsWith("$share/"))return false;
            if(topicFilter.IsNotMatch(@"^\$share\/[a-z0-9]+\/[a-z0-9\/\+#]+"))return false;
            return true;
        }
        /// <summary>
        /// 过滤器是否匹配主题
        /// </summary>
        /// <param name="topicFilter">过滤器</param>
        /// <param name="topicName">主题</param>
        /// <param name="isWildcard">是否支持野匹配</param>
        /// <returns></returns>
        public static bool IsContainsTopic(string topicFilter, string topicName, bool isWildcard = true)
        {
            var tFilter = topicFilter;
            if(IsSharedTopicFilter(tFilter))
            {
                tFilter = tFilter.RemovePattern(@"^\$share\/[a-z0-9]+\/");
            }
            if (!isWildcard) return tFilter == topicName;
            if (tFilter == "#" || tFilter == topicName) return true;
            if (tFilter.EndsWith("#"))
            {
                var topic = tFilter.ReplacePattern(@"\/#$", @"");
                if (topicName.StartsWith(topic)) return true;
            }
            if (tFilter == "+" && !topicName.Contains("/")) return true;
            if (tFilter.Contains("+"))
            {
                var topic = tFilter.ReplacePattern(@"+", @"[^/]*").TrimEnd('#');
                if (topicName.IsMatch(@"^" + topic)) return true;
            }
            return false;
        }
        #endregion
    }
}