# XiaoFeng.Mqtt

![fayelf](https://user-images.githubusercontent.com/16105174/197918392-29d40971-a8a2-4be4-ac17-323f1d0bed82.png)

![GitHub top language](https://img.shields.io/github/languages/top/zhuovi/xiaofeng.mqtt?logo=github)
![GitHub License](https://img.shields.io/github/license/zhuovi/xiaofeng.mqtt?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/xiaofeng.mqtt?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/xiaofeng.mqtt?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/xiaofeng.mqtt?label=dev%20nuget&logo=nuget)

Nuget：XiaoFeng.Mqtt

| QQ群号 | QQ群 | 公众号 |
| :----:| :----: | :----: |
| 748408911  | ![QQ 群](https://user-images.githubusercontent.com/16105174/198058269-0ea5928c-a2fc-4049-86da-cca2249229ae.png) | ![畅聊了个科技](https://user-images.githubusercontent.com/16105174/198059698-adbf29c3-60c2-4c76-b894-21793b40cf34.jpg) |

源码： https://github.com/zhuovi/xiaofeng.mqtt

教程： https://www.yuque.com/fayelf/xiaofeng

XiaoFeng.Mqtt中间件,支持.NET框架、.NET内核和.NET标准库,一种非常方便操作的客户端工具。实现了MQTT客户端，MQTT服务端,同时支持TCP，WebSocket连接。支持协议版本3.0.0,3.1.0,5.0.0

## 感谢支持

| 名称 | LOGO |
| :----:| :----: |
| JetBrains | [![JetBrains](https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg?_ga=2.18748729.1472960975.1710982503-1993260277.1703834590&_gl=1*1o75dn2*_ga*MTk5MzI2MDI3Ny4xNzAzODM0NTkw*_ga_9J976DJZ68*MTcxMDk4MjUwMi43LjEuMTcxMDk4NDUwOC4zOC4wLjA.)](https://jb.gg/OpenSourceSupport) |
| Visual Studio | [![Visual Studio](https://visualstudio.microsoft.com/wp-content/uploads/2021/10/Product-Icon.svg)](https://visualstudio.microsoft.com/) |

## XiaoFeng.Mqtt

XiaoFeng.Mqtt generator with [XiaoFeng.Mqtt](https://github.com/zhuovi/XiaoFeng.Mqtt).

## Install

.NET CLI

```
$ dotnet add package XiaoFeng.Mqtt --version 1.0.4
```

Package Manager

```
PM> Install-Package XiaoFeng.Mqtt --Version 1.0.4
```

PackageReference

```
<PackageReference Include="XiaoFeng.Mqtt" Version="1.0.4" />
```

Paket CLI

```
> paket add XiaoFeng.Mqtt --version 1.0.4
```

Script & Interactive

```
> #r "nuget: XiaoFeng.Mqtt, 1.0.4"
```

Cake

```
// Install XiaoFeng.Mqtt as a Cake Addin
#addin nuget:?package=XiaoFeng.Mqtt&version=1.0.4

// Install XiaoFeng.Mqtt as a Cake Tool
#tool nuget:?package=XiaoFeng.Mqtt&version=1.0.4
```

# XiaoFeng 类库包含库
| 命名空间 | 所属类库 | 开源状态 | 说明 | 包含功能 |
| :----| :---- | :---- | :----: | :---- |
| XiaoFeng.Prototype | XiaoFeng.Core | :white_check_mark: | 扩展库 | ToCase 类型转换<br/>ToTimestamp,ToTimestamps 时间转时间戳<br/>GetBasePath 获取文件绝对路径,支持Linux,Windows<br/>GetFileName 获取文件名称<br/>GetMatch,GetMatches,GetMatchs,IsMatch,ReplacePatten,RemovePattern 正则表达式操作<br/> |
| XiaoFeng.Net | XiaoFeng.Net | :white_check_mark: | 网络库 | XiaoFeng网络库，封装了Socket客户端，服务端（Socket,WebSocket），根据当前库可轻松实现订阅，发布等功能。|
| XiaoFeng.Http | XiaoFeng.Core | :white_check_mark: | 模拟请求库 | 模拟网络请求 |
| XiaoFeng.Data | XiaoFeng.Core | :white_check_mark: | 数据库操作库 | 支持SQLSERVER,MYSQL,ORACLE,达梦,SQLITE,ACCESS,OLEDB,ODBC等数十种数据库 |
| XiaoFeng.Cache | XiaoFeng.Core | :white_check_mark: | 缓存库 |  内存缓存,Redis,MemcachedCache,MemoryCache,FileCache缓存 |
| XiaoFeng.Config | XiaoFeng.Core | :white_check_mark: | 配置文件库 | 通过创建模型自动生成配置文件，可为xml,json,ini文件格式 |
| XiaoFeng.Cryptography | XiaoFeng.Core | :white_check_mark: | 加密算法库 | AES,DES,RSA,MD5,DES3,SHA,HMAC,RC4加密算法 |
| XiaoFeng.Excel | XiaoFeng.Excel | :white_check_mark: | Excel操作库 | Excel操作，创建excel,编辑excel,读取excel内容，边框，字体，样式等功能  |
| XiaoFeng.Ftp | XiaoFeng.Ftp | :white_check_mark: | FTP请求库 | FTP客户端 |
| XiaoFeng.IO | XiaoFeng.Core | :white_check_mark: | 文件操作库 | 文件读写操作 |
| XiaoFeng.Json | XiaoFeng.Core | :white_check_mark: | Json序列化，反序列化库 | Json序列化，反序列化库 |
| XiaoFeng.Xml | XiaoFeng.Core | :white_check_mark: | Xml序列化，反序列化库 | Xml序列化，反序列化库 |
| XiaoFeng.Log | XiaoFeng.Core | :white_check_mark: | 日志库 | 写日志文件,数据库 |
| XiaoFeng.Memcached | XiaoFeng.Memcached | :white_check_mark: | Memcached缓存库 | Memcached中间件,支持.NET框架、.NET内核和.NET标准库,一种非常方便操作的客户端工具。实现了Set,Add,Replace,PrePend,Append,Cas,Get,Gets,Gat,Gats,Delete,Touch,Stats,Stats Items,Stats Slabs,Stats Sizes,Flush_All,Increment,Decrement,线程池功能。|
| XiaoFeng.Redis | XiaoFeng.Redis | :white_check_mark: | Redis缓存库 | Redis中间件,支持.NET框架、.NET内核和.NET标准库,一种非常方便操作的客户端工具。实现了Hash,Key,String,ZSet,Stream,Log,List,订阅发布,线程池功能; |
| XiaoFeng.Threading | XiaoFeng.Core | :white_check_mark: | 线程库 | 线程任务,线程队列 |
| XiaoFeng.Mvc | XiaoFeng.Mvc | :x: | 低代码WEB开发框架 | .net core 基础类，快速开发CMS框架，真正的低代码平台，自带角色权限，WebAPI平台，后台管理，可托管到服务运行命令为:应用.exe install 服务名 服务说明,命令还有 delete 删除 start 启动  stop 停止。 |
| XiaoFeng.Proxy | XiaoFeng.Proxy | :white_check_mark: | 代理库 | 开发中 |
| XiaoFeng.TDengine | XiaoFeng.TDengine | :white_check_mark: | TDengine 客户端 | 开发中 |
| XiaoFeng.GB28181 | XiaoFeng.GB28181 | :white_check_mark: | 视频监控库，SIP类库，GB28181协议 | 开发中 |
| XiaoFeng.Onvif | XiaoFeng.Onvif | :white_check_mark: | 视频监控库Onvif协议 | XiaoFeng.Onvif 基于.NET平台使用C#封装Onvif常用接口、设备、媒体、云台等功能， 拒绝WCF服务引用动态代理生成wsdl类文件 ， 使用原生XML扩展标记语言封装参数，所有的数据流向都可控。 |
| FayElf.Plugins.WeChat | FayElf.Plugins.WeChat | :white_check_mark: | 微信公众号，小程序类库 | 微信公众号，小程序类库。 |
| XiaoFeng.Mqtt | XiaoFeng.Mqtt | :white_check_mark: | MQTT协议 | XiaoFeng.Mqtt中间件,支持.NET框架、.NET内核和.NET标准库,一种非常方便操作的客户端工具。实现了MQTT客户端，MQTT服务端,同时支持TCP，WebSocket连接。支持协议版本3.0.0,3.1.0,5.0.0。 |
| XiaoFeng.Modbus | XiaoFeng.Modbus | :white_check_mark: | MODBUS协议 | MODBUS协议,支持RTU、ASCII、TCP三种方式进行通信，自动离线保存服务端数据 |
| XiaoFeng.DouYin | XiaoFeng.DouYin | :white_check_mark: | 抖音开放平台SDK | 抖音开放平台接口 |
| XiaoFeng.KuaiShou | XiaoFeng.KuaiShou | :white_check_mark: | 快手开放平台SDK | 快手开放平台接口 |
| XiaoFeng.Mvc.AdminWinDesk | XiaoFeng.Mvc.AdminWinDesk | :white_check_mark: | XiaoFeng.Mvc后台皮肤 | 模仿windows桌面后台皮肤 |
| FayElf.Cube.Blog | FayElf.Cube.Blog | :white_check_mark: | XiaoFeng.Mvc开发的技术博客 | 使用低代码开发框架（XiaoFeng.Mvc）+Windows后台皮肤(XiaoFeng.Mvc.AdminWinDesk)，开发的一个博客平台。 |
| XiaoFeng.Ofd | XiaoFeng.Ofd | :white_check_mark: | OFD读写库 | OFD 读写处理库，支持文档的生成、文档编辑、文档批注、数字签名、文档合并、文档拆分、文档转换至PDF、文档查询等功能。 |


# XiaoFeng.Mqtt

Mqtt提供了友好的访问API。

## 基本使用方法

Mqtt连接串 

```csharp
mqtt://username:7092734@127.0.0.1:1883?ConnectionTimeout=3000&ReadTimeout=3000&WriteTimeout=3000
[<protocol>]://[[<username>:<password>@]<host>:<port>][?<p1>=<v1>[&<p2>=<v2>]]
|----------|---|-----------|-----------|------|------|-----------------------|
| protocol |   | username  | password  | host | port |  params               |

```
protocol    支持mqtt,mqtts,ws,wss,tcp

username   用户名

7092734	    密码

127.0.0.1	主机

1883		端口

ConnectionTimeout	连接超时时长

ReadTimeout		    读取数据超时时长

WriteTimeout		    发送数据超时时长

最小的连接串是：mqtt://127.0.0.1

## 实例化一个Mqtt对象

## MQTT 客户端

下边代码直接拷贝到程序中即可使用,修改一下服务器连接串或账号密码等即可.

```csharp
var client = new MqttClient("mqtt://admin:123456@127.0.0.1:1883");
client.ClientOptions = new MqttClientOptions
{
    UserName = "admin",
    Password = "123456",
    ProtocolVersion = MqttProtocolVersion.V500//可指定相关版本
};
client.OnStarted += o =>
{
    w("已连接.");
};
client.OnStoped += o =>
{
    w("已停止.");
};
client.OnError += (o, e) =>
{
    //错误消息
    w(e);
};
client.OnMessage += e =>
{
    //除订阅消息以外的所有消息
    var msg = "Unknown error.";
    if (e.ResultType == ResultType.Success)
    {
        if (e.Message.IsNullOrEmpty())
        {
            if (e.MqttPacket != null) msg = e.MqttPacket.ToString();
            else msg = "Unknown packet.";
        }
        else
            msg = e.Message;
    }
    else
    {
        msg = e.Message.IsNullOrEmpty() ? "" : e.Message;
    }
    w(msg);
};
client.OnPublishMessage += e =>
{
    //专一接收所订阅的消息
    var c = client;
    w($"Publish Message: {e}");
};
//开启连接服务端
await client.ConnectAsync().ConfigureAwait(false);
//订阅主题
await client.SubscributeAsync("a/b").ConfigureAwait(false);
//向指定的主题发送消息
await client.PublishAsync("a/b", "Hello World").ConfigureAwait(false);

static void w(string msg) => Console.WriteLine($"{DateTime.Now.ToTimeStamp()}: {msg}");
```
## MQTT 服务端

下边代码直接拷贝到程序中即可使用,修改一下账号密码及端口

```csharp
var server = new MqttServer(new IPEndPoint(IPAddress.Any, 1883));
//添加服务器凭证
server.AddCredential("admin", "123456", "");
server.SameClientIdMode = SamClientIdMode.Refused;
server.OnStarted += o =>
{
    //服务器启动消息
    w("ELF (MQTT) server version 1.0.0 starting.");
    w("Config loaded from elfmqtt.conf.");
    w($"Listen {server.ServerOptions.EndPoint} mqtt://{server.ServerOptions.EndPoint}");
};
server.OnStoped += o =>
{
    //服务器停止消息
    w("ELF MQTT Server Version 1.0.0 stoped.");
};
server.OnConnected += o =>
{
    //新连接消息
    w($"New connection from {o.EndPoint} on port {server.ServerOptions.EndPoint.Port}");
};
server.OnError += (o, m) =>
{
    //错误消息
    w($"Error: {m}");
};
server.OnDisconnected += o =>
{
    //客户端断开连接消息
    var data = o.GetClientData();
    var address = "";
    if (data != null && data.ConnectPacket != null)
        address = data.ConnectPacket.ClientId;
    address += $"[{o.EndPoint}]";
    w($"Client {address} disconnected.");
};
server.OnMessage += (o, e) =>
{
    //接收消息
    var msg = "Unknown error.";
    if (e.ResultType == ResultType.Success)
    {
        if (e.Message.IsNullOrEmpty())
        {
            if (e.MqttPacket != null) msg = e.MqttPacket.ToString();
            else msg = "Unknown packet.";
        }
        else
            msg = e.Message;
    }
    else
    {
        msg = e.Message.IsNullOrEmpty() ? "" : e.Message;
    }
    w(msg);
};

server.Start();

static void w(string msg) => Console.WriteLine($"{DateTime.Now.ToTimeStamp()}: {msg}");
```



# 作者介绍

* 网址 : https://www.eelf.cn
* QQ : 7092734
* EMail : jacky@eelf.cn


# 版本更新

## 2025-01-02   v 1.0.4

1.增加服务端配置 SameClientIdMode 模式,当客户端ID与已连接的客户端ID相同时的处理方式;

2.优化MqttClient，在断网或连接失败的情况下，优化连接网络;

3.优化输出包字节时，优化包类型不匹配的问题;

4.优化自动连接;

## 2024-02-19   v 1.0.3

1.优化服务端,客户端部分功能;

2.优化服务端发送Disconnected时客户端解释包bug;

3.优化WillFlag为0时连接包直接设备WillRetain为0;

4.优化从网址中提取密码时赋值的bug;

5.优化解包失败时丢弃包;

6.MqttClient优化服务端拒绝连接时不再自动连接;

## 2023-12-29   v 1.0.2

1.优化服务端分发消息时的效率;

2.优化解包失败时放弃当前包;

3.优化MqttServerCredential中账号或密码为空时的处理;

## 2023-10-18   v 1.0.1

增加粘包分包处理;

## 2023-10-14   v 1.0.0

全新功能的MQTT客户端服务端,支持3.0.0,3.1.0,5.0.0协议版本
