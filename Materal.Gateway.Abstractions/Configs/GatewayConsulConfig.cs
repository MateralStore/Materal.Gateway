namespace Materal.Gateway.Abstractions.Configs;

/// <summary>
/// 网关Consul配置
/// </summary>
public class GatewayConsulConfig
{
    /// <summary>
    /// Consul地址
    /// </summary>
    public string Url { get; set; } = "http://127.0.0.1:8500/";
    /// <summary>
    /// 健康检查间隔
    /// </summary>
    public int Interval { get; set; } = 10;
    /// <summary>
    /// Token
    /// </summary>
    public string? Token { get; set; }
}
