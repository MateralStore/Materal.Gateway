using Microsoft.AspNetCore.Builder;

namespace Materal.Gateway.Extensions;

/// <summary>
/// 
/// </summary>
public static class WebApplicationExtension
{
    /// <summary>
    /// 代理中间件
    /// </summary>
    /// <param name="app"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static WebApplication UseGateway(this WebApplication app, Action<GatewayAppOptions>? options = null)
    {
        GatewayAppOptions appOptions = new(app);
        options?.Invoke(appOptions);
        app.MapReverseProxy(builder =>
        {
            appOptions.ReverseConfigApp?.Invoke(builder);
        });
        return app;
    }
}
