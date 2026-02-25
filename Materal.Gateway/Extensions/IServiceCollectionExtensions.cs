using Materal.Gateway.ConfigProviders;

namespace Materal.Gateway.Extensions;

/// <summary>
/// 容器扩展
/// </summary>
public static partial class IServiceCollectionExtensions
{
    /// <summary>
    /// 添加网关
    /// </summary>
    /// <param name="service"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="GatewayException"></exception>
    public static IServiceCollection AddGateway(this IServiceCollection service, Action<GatewayOptions>? options = null)
    {
        service.AddHttpContextAccessor();
        IReverseProxyBuilder reverseProxyBuilder = service.AddReverseProxy();
        service.AddSingleton<IProxyConfigProvider, ConsulIProxyConfigProvider>();
        GatewayOptions option = new();
        if (options != null)
        {
            options.Invoke(option);
            option.ReverseProxyBuilderInject?.Invoke(reverseProxyBuilder);
        }
        return service;
    }
}
