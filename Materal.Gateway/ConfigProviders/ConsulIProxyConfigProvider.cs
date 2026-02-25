using Consul;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using DestinationConfig = Yarp.ReverseProxy.Configuration.DestinationConfig;
using RouteConfig = Yarp.ReverseProxy.Configuration.RouteConfig;

namespace Materal.Gateway.ConfigProviders;

/// <summary>
/// Consul代理配置供应者
/// </summary>
public class ConsulIProxyConfigProvider(IOptionsMonitor<ConsulOptions> consulOptions) : IProxyConfigProvider
{
    private volatile ConsulIProxyConfig _config = new([], []);
    private static readonly SemaphoreSlim _reloadSemaphore = new(1, 1);

    /// <summary>
    /// Consul客户端
    /// </summary>
    public ConsulClient ConsulClient { get; } = new(config => config.Address = new Uri(consulOptions.CurrentValue.Host));

    /// <inheritdoc/>
    public IProxyConfig GetConfig() => _config;

    private void Update(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
    {
        ConsulIProxyConfig newConfig = new(routes, clusters);
        UpdateInternal(newConfig);
    }

    private void UpdateInternal(ConsulIProxyConfig newConfig)
    {
        ConsulIProxyConfig oldConfig = Interlocked.Exchange(ref _config, newConfig);
        oldConfig.SignalChange();
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <returns></returns>
    public async Task UpdateAsync()
    {
        await _reloadSemaphore.WaitAsync();
        try
        {
            ConsulIProxyConfig _oldConfig = _config;
            List<RouteConfig> routes = [];
            List<ClusterConfig> clusters = [];
            QueryResult<Dictionary<string, string[]>> servicesResult = await ConsulClient.Catalog.Services();
            foreach (KeyValuePair<string, string[]> service in servicesResult.Response)
            {
                if (service.Key == "consul") continue;
                QueryResult<CatalogService[]> serviceResult = await ConsulClient.Catalog.Service(service.Key);
                // 创建目标地址字典
                Dictionary<string, DestinationConfig> destinations = [];
                int destinationIndex = 0;
                foreach (CatalogService item in serviceResult.Response)
                {
                    if (!item.ServiceMeta.TryGetValue("ServiceUrl", out string? serviceUrl) || string.IsNullOrWhiteSpace(serviceUrl)) continue;
                    destinations[$"destination{destinationIndex++}"] = new DestinationConfig
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
            Update(routes, clusters);
        }
        finally
        {
            _reloadSemaphore.Release();
        }
    }

    private sealed class ConsulIProxyConfig : IProxyConfig
    {
        private readonly CancellationTokenSource _cts = new();

        public ConsulIProxyConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters) : this(routes, clusters, Guid.NewGuid().ToString())
        { }

        public ConsulIProxyConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters, string revisionId)
        {
            ArgumentNullException.ThrowIfNull(revisionId);
            RevisionId = revisionId;
            Routes = routes;
            Clusters = clusters;
            ChangeToken = new CancellationChangeToken(_cts.Token);
        }

        public string RevisionId { get; }

        public IReadOnlyList<RouteConfig> Routes { get; }

        public IReadOnlyList<ClusterConfig> Clusters { get; }

        public IChangeToken ChangeToken { get; }

        internal void SignalChange() => _cts.Cancel();
    }
}
