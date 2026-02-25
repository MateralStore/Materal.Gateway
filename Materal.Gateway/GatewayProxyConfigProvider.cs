namespace Materal.Gateway;

/// <summary>
/// Gateway 路由映射提供器
/// </summary>
public class GatewayProxyConfigProvider : IProxyConfigProvider
{
    private volatile GatewayProxyConfig _config = new();
    /// <inheritdoc/>
    public IProxyConfig GetConfig() => _config;
    /// <inheritdoc/>
    public void Refresh(IReadOnlyList<RouteConfig> routeConfigs, IReadOnlyList<ClusterConfig> clusterConfigs)
    {
        GatewayProxyConfig oldConfig = _config;
        _config = new GatewayProxyConfig(routeConfigs, clusterConfigs);
        oldConfig.SignalStop();
    }
}
