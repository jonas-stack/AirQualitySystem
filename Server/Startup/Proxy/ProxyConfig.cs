using System.Net;
using WebSocketProxy;
using Host = WebSocketProxy.Host;

namespace Startup.Proxy;

public class ProxyConfig : IProxyConfig
{
    public void StartProxyServer(int publicPort, int restPort, int wsPort)
    {
        var proxyConfiguration = new TcpProxyConfiguration
        {
            PublicHost = new Host
            {
                IpAddress = IPAddress.Parse("0.0.0.0"),
                Port = publicPort
            },
            HttpHost = new Host
            {
                IpAddress = IPAddress.Loopback,
                Port = restPort
            },
            WebSocketHost = new Host
            {
                IpAddress = IPAddress.Loopback,
                Port = wsPort
            }
        };
        new TcpProxyServer(proxyConfiguration).Start();
    }
}