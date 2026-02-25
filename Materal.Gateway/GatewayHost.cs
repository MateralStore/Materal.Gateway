namespace Materal.Gateway;

/// <summary>
/// 网关主机
/// </summary>
public class GatewayHost : IGatewayHost
{
    /// <summary>
    /// 网关配置
    /// </summary>
    protected readonly IOptionsMonitor<GatewayConfig> Config;
    private readonly GatewayProxyConfigProvider _yarpConfigProvider;
    private readonly IDisposable? _optionsReloadToken;
    /// <summary>
    /// 日志
    /// </summary>
    protected readonly ILogger? Logger;
    /// <summary>
    /// 构造方法
    /// </summary>
    public GatewayHost(IOptionsMonitor<GatewayConfig> config, GatewayProxyConfigProvider yarpConfigProvider, ILoggerFactory? loggerFactory = null)
    {
        Config = config;
        _yarpConfigProvider = yarpConfigProvider;
        _optionsReloadToken = config.OnChange(ConfigChange);
        Logger = loggerFactory?.CreateLogger(GetType().Name);
    }
    /// <inheritdoc/>
    public virtual async Task StartAsync()
    {
        RefreshConfig();
        await Task.CompletedTask;
    }
    /// <inheritdoc/>
    public virtual async Task StopAsync()
    {
        _optionsReloadToken?.Dispose();
        await Task.CompletedTask;
    }
    private void ConfigChange(GatewayConfig config, string? name) => RefreshConfig();
    /// <summary>
    /// 刷新配置
    /// </summary>
    protected virtual void RefreshConfig()
    {
        RefreshYarpConfig();
    }
    /// <summary>
    /// 刷新Yarp配置
    /// </summary>
    protected virtual void RefreshYarpConfig()
    {
        (List<RouteConfig> routeConfigs, List<ClusterConfig> clusterConfigs) = GetYarpConfigs();
        _yarpConfigProvider.Refresh(routeConfigs, clusterConfigs);
    }
    private (List<RouteConfig> routeConfigs, List<ClusterConfig> clusterConfigs) GetYarpConfigs()
    {
        List<RouteConfig> routeConfigs = [];
        List<ClusterConfig> clusterConfigs = [];
        foreach (GatewayRouteConfig config in Config.CurrentValue.Routes)
        {
            try
            {
                (RouteConfig routeConfig, ClusterConfig clusterConfig) = GetYarpConfig(config);
                routeConfigs.Add(routeConfig);
                clusterConfigs.Add(clusterConfig);
            }
            catch (Exception ex)
            {
                Logger?.LogWarning(ex, "配置转换失败");
                continue;
            }
        }
        return (routeConfigs, clusterConfigs);
    }
    private (RouteConfig routeConfig, ClusterConfig clusterConfig) GetYarpConfig(GatewayRouteConfig config)
    {
        #region 组织ClusterConfig
        Dictionary<string, DestinationConfig> destinations = GetDestinationConfigs(config);
        ForwarderRequestConfig forwarderRequestConfig;
        if (config.HttpVersion != "2")
        {
            forwarderRequestConfig = new()
            {
                Version = new Version(config.HttpVersion),
                VersionPolicy = HttpVersionPolicy.RequestVersionExact,
                ActivityTimeout = TimeSpan.FromMinutes(config.ActivityTimeout)
            };
        }
        else
        {
            forwarderRequestConfig = new()
            {
                ActivityTimeout = TimeSpan.FromMinutes(config.ActivityTimeout)
            };
        }
        ClusterConfig clusterConfig = new()
        {
            ClusterId = $"{config.Name}Cluster",
            LoadBalancingPolicy = config.LoadBalancerOptions,
            Destinations = destinations,
            HttpClient = new HttpClientConfig { DangerousAcceptAnyServerCertificate = true },
            HttpRequest = forwarderRequestConfig
        };
        #endregion
        #region 组织RouteConfig
        List<IReadOnlyDictionary<string, string>>? transforms = null;
        if (!string.IsNullOrWhiteSpace(config.DownstreamPathTemplate))
        {
            transforms = [new Dictionary<string, string>() { { "PathPattern", config.DownstreamPathTemplate } }];
        }
        RouteConfig routeConfig = new()
        {
            RouteId = $"{config.Name}Route",
            ClusterId = clusterConfig.ClusterId,
            Match = new RouteMatch
            {
                Path = config.UpstreamPathTemplate
            },
            Transforms = transforms
        };
        #endregion
        return (routeConfig, clusterConfig);
    }
    /// <summary>
    /// 获取目标配置
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    protected virtual Dictionary<string, DestinationConfig> GetDestinationConfigs(GatewayRouteConfig config)
    {
        Dictionary<string, DestinationConfig> destinations = new(StringComparer.OrdinalIgnoreCase);
        if (config.DownstreamUrls is not null && config.DownstreamUrls.Count > 0)
        {
            for (int i = 0; i < config.DownstreamUrls.Count; i++)
            {
                string url = config.DownstreamUrls[i];
                DestinationConfig destinationConfig = new()
                {
                    Address = url
                };
                destinations.Add($"{config.Name}Destination{i}", destinationConfig);
            }
        }
        return destinations;
    }
}
