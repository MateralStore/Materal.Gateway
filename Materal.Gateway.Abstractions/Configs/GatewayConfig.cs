namespace Materal.Gateway.Abstractions.Configs;

/// <summary>
/// 网关配置
/// </summary>
public class GatewayConfig
{
    /// <summary>
    /// Consul配置
    /// </summary>
    public GatewayConsulConfig? Consul { get; set; }
    /// <summary>
    /// 集群配置
    /// </summary>
    public List<GatewayRouteConfig> Routes { get; set; } = [];
}
