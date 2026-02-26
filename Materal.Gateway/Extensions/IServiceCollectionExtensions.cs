using Materal.Gateway.ConfigProviders;
using Microsoft.Extensions.Configuration;

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
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddGateway(this IServiceCollection service, IConfiguration? configuration, Action<GatewayOptions>? options = null)
    {
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));
        service.AddHttpContextAccessor();
        IReverseProxyBuilder reverseProxyBuilder = service.AddReverseProxy();
        service.AddSingleton<ConsulIProxyConfigProvider>();
        service.AddSingleton((Func<IServiceProvider, IProxyConfigProvider>)((IServiceProvider s) => s.GetRequiredService<ConsulIProxyConfigProvider>()));
        service.AddHostedService<ConsulIProxyConfigHostedService>();
        service.Configure<ConsulOptions>(configuration.GetSection("Consul"));
        GatewayOptions option = new();
        if (options != null)
        {
            options.Invoke(option);
            option.ReverseProxyBuilderInject?.Invoke(reverseProxyBuilder);
        }
        return service;
    }
}
