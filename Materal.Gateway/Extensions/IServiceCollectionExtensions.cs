using Materal.Gateway.Configs;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
    public static IServiceCollection AddGateway(this IServiceCollection service, Action<GatewayOptions> options)
        => service.AddGateway<GatewayHost>(options);
    /// <summary>
    /// 添加网关
    /// </summary>
    /// <typeparam name="THost"></typeparam>
    /// <param name="service"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="GatewayException"></exception>
    public static IServiceCollection AddGateway<THost>(this IServiceCollection service, Action<GatewayOptions> options)
        where THost : class, IGatewayHost
    {
        service.AddHttpContextAccessor();
        IReverseProxyBuilder reverseProxyBuilder = service.AddReverseProxy();
        GatewayProxyConfigProvider provider = new();
        service.AddSingleton<IProxyConfigProvider>(provider);
        service.AddSingleton(provider);
        GatewayOptions option = new();
        if (options != null)
        {
            options.Invoke(option);
            if (option.Configuration is null) throw new GatewayException("获取配置对象失败");
            if (option.Configuration is IConfigurationManager configurationManager)
            {
                configurationManager.AddJsonFile(option.ConfigFilePath, false, true);
            }
            GatewayConfigManager.ConfigFilePath = option.ConfigFilePath;
            IConfigurationSection configurationSection = option.Configuration.GetSection("Gateway");
            if (configurationSection is not null)
            {
                service.Configure<GatewayConfig>(configurationSection);
            }
            else
            {
                service.Configure<GatewayConfig>(option.Configuration);
            }
            option.ReverseProxyBuilderInject?.Invoke(reverseProxyBuilder);
        }
        service.AddHostedService<GatewayHostedService>();
        service.TryAddSingleton<IGatewayHost, THost>();
        service.TryAddSingleton<IGatewayConfigManager, GatewayConfigManager>();
        return service;
    }
}
