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
}
