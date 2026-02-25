namespace Materal.Gateway.Abstractions.Configs;

/// <summary>
/// 网关路由Consul配置
/// </summary>
public class GatewayRouteConsulConfig
{
    /// <summary>
    /// 服务名称
    /// </summary>
    [Required]
    public string ServiceName { get; set; } = string.Empty;
    /// <summary>
    /// Http模式
    /// </summary>
    public string Schema { get; set; } = "http";
}
