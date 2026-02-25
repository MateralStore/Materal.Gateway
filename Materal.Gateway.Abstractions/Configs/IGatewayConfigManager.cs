namespace Materal.Gateway.Abstractions.Configs;

/// <summary>
/// 网关配置管理器
/// </summary>
public interface IGatewayConfigManager
{
    /// <summary>
    /// 添加路由
    /// </summary>
    /// <param name="gatewayRouteConfig"></param>
    Task AddRouteConfigAsync(GatewayRouteConfig gatewayRouteConfig);
    /// <summary>
    /// 修改路由
    /// </summary>
    /// <param name="gatewayRouteConfig"></param>
    Task EditRouteConfigAsync(GatewayRouteConfig gatewayRouteConfig);
    /// <summary>
    /// 删除路由
    /// </summary>
    /// <param name="routeName"></param>
    Task DeleteRouteConfigAsync(string routeName);
    /// <summary>
    /// 获取路由
    /// </summary>
    /// <param name="routeName"></param>
    /// <returns></returns>
    GatewayRouteConfig GetRouteConfig(string routeName);
    /// <summary>
    /// 获取所有路由
    /// </summary>
    /// <returns></returns>
    List<GatewayRouteConfig> GetAllRouteConfigs();
}
