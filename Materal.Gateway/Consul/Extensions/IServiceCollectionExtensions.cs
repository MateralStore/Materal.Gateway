using Materal.Gateway.Extensions;

namespace Materal.Gateway.Consul.Extensions;

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
    public static IServiceCollection AddConsulGateway(this IServiceCollection service, Action<GatewayOptions> options)
    {
        service.AddGateway<ConsulGatewayHost>(options);
        return service;
    }
}
