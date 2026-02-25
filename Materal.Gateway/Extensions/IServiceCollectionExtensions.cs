using Materal.Gateway.ConfigProviders;

namespace Materal.Gateway.Extensions;

/// <summary>
/// 容器扩展
/// </summary>
public static partial class IServiceCollectionExtensions
{
    ///// <summary>
    ///// 添加网关
    ///// </summary>
    ///// <param name="service"></param>
    ///// <param name="options"></param>
    ///// <returns></returns>
    //public static IServiceCollection AddGateway(this IServiceCollection service, Action<GatewayOptions> options)
    //    => service.AddGateway<GatewayHost>(options);
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
        ConsulIProxyConfigProvider provider = new();
        service.AddSingleton<IProxyConfigProvider>(provider);
        //service.AddSingleton(provider);
        GatewayOptions option = new();
        if (options != null)
        {
            options.Invoke(option);
            option.ReverseProxyBuilderInject?.Invoke(reverseProxyBuilder);
        }
        return service;
    }
}
