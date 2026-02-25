namespace Materal.Gateway.Abstractions.Configs;

/// <summary>
/// 路由转发配置
/// </summary>
public class GatewayRouteConfig
{
    /// <summary>
    /// 名称
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 空闲超时时长
    /// </summary>
    public double ActivityTimeout { get; set; } = 5;
    /// <summary>
    /// 上游路径模版
    /// </summary>
    [Required]
    public string UpstreamPathTemplate { get; set; } = string.Empty;
    /// <summary>
    /// 下游路径模版
    /// </summary>
    [Required]
    public string DownstreamPathTemplate { get; set; } = string.Empty;
    /// <summary>
    /// 下端服务地址
    /// </summary>
    public List<string>? DownstreamUrls { get; set; }
    /// <summary>
    /// Consul配置
    /// </summary>
    public GatewayRouteConsulConfig? Consul { get; set; }
    /// <summary>
    /// http版本
    /// </summary>
    public string HttpVersion { get; set; } = "2";
    /// <summary>
    /// 负载均衡策略
    /// </summary>
    public string LoadBalancerOptions { get; set; } = "PowerOfTwoChoices";
}
