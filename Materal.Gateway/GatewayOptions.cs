namespace Materal.Gateway;

/// <summary>
/// 网关选项
/// </summary>
public class GatewayOptions
{
    /// <summary>
    /// 反向代理构建器
    /// </summary>
    public Action<IReverseProxyBuilder>? ReverseProxyBuilderInject { get; set; }
    /// <summary>
    /// 配置对象
    /// </summary>
    public IConfiguration? Configuration { get; set; }
    /// <summary>
    /// 配置文件路径
    /// </summary>
    public string ConfigFilePath { get; set; } = "GatewayConfig.json";
}
