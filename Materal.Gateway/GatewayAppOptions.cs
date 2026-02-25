using Microsoft.AspNetCore.Builder;

namespace Materal.Gateway;

/// <summary>
/// Gateway应用程序选项
/// </summary>
public class GatewayAppOptions(WebApplication app)
{
    /// <summary>
    /// 应用程序
    /// </summary>
    public WebApplication App { get; } = app;
    /// <summary>
    /// MapReverseProxy 参数
    /// </summary>
    public Action<IReverseProxyApplicationBuilder>? ReverseConfigApp { get; set; }
}
