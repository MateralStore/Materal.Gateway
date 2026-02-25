using Consul;
using Microsoft.Extensions.Options;
using RouteConfig = Yarp.ReverseProxy.Configuration.RouteConfig;
using YarpDestinationConfig = Yarp.ReverseProxy.Configuration.DestinationConfig;

namespace Materal.Gateway.ConfigProviders;

/// <summary>
/// Consul代理配置供应者
/// </summary>
public class ConsulIProxyConfigProvider : IProxyConfigProvider
{
    private GatewayProxyConfig _config = new();
    private readonly ConsulClient _consulClient;

    /// <summary>
    /// 构造方法
    /// </summary>
    public ConsulIProxyConfigProvider(IOptionsMonitor<ConsulOptions> consulOptions)
    {
        _consulClient = new(config => config.Address = new Uri(consulOptions.CurrentValue.Host));
        Task task = Task.Run(ReloadConfigAsync);
        Task.WaitAll(task);
    }

    /// <inheritdoc/>
    public IProxyConfig GetConfig() => _config;

    private async Task ReloadConfigAsync()
    {
        GatewayProxyConfig _oldConfig = _config;
        List<RouteConfig> routes = [];
        List<ClusterConfig> clusters = [];
        QueryResult<Dictionary<string, string[]>> servicesResult = await _consulClient.Catalog.Services();
        foreach (KeyValuePair<string, string[]> service in servicesResult.Response)
        {
            if (service.Key == "consul") continue;
            QueryResult<CatalogService[]> serviceResult = await _consulClient.Catalog.Service(service.Key);
            // 创建目标地址字典
            Dictionary<string, YarpDestinationConfig> destinations = [];
            int destinationIndex = 0;
            foreach (CatalogService item in serviceResult.Response)
            {
                if (!item.ServiceMeta.TryGetValue("ServiceUrl", out string? serviceUrl) || string.IsNullOrWhiteSpace(serviceUrl)) continue;
                destinations[$"destination{destinationIndex++}"] = new YarpDestinationConfig
                {
                    Address = serviceUrl
                };
            }
            // 创建集群配置
            ClusterConfig cluster = new()
            {
                ClusterId = service.Key,           // 集群ID，使用服务名称作为唯一标识
                Destinations = destinations,       // 目标地址列表，包含该服务的所有实例
                LoadBalancingPolicy = "RoundRobin" // 负载均衡策略：轮询
            };
            clusters.Add(cluster);
            // 创建路由配置
            RouteConfig route = new()
            {
                RouteId = $"{service.Key}-route",  // 路由唯一ID
                ClusterId = service.Key,           // 关联到对应的集群
                Match = new RouteMatch
                {
                    Path = $"/{service.Key}/{{**catch-all}}"  // 匹配路径模式
                },
                Transforms =
                [
                    new Dictionary<string, string>
                    {
                        ["PathPattern"] = "/{**catch-all}"
                    }
                ]
            };
            routes.Add(route);
        }
        _config = new(routes, clusters);
        _oldConfig.SignalStop();
    }
}
