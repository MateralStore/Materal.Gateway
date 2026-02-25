using Materal.Gateway.Abstractions.Configs;
using Microsoft.AspNetCore.Mvc;

namespace Materal.Gateway.Application.Controllers;

/// <summary>
/// 网关配置控制器
/// </summary>
public class GatewayConfigController(IGatewayConfigManager gatewayConfigManager) : ControllerBase
{
    /// <inheritdoc/>
    public async Task<ResultModel> AddRouteConfigAsync(GatewayRouteConfig gatewayRouteConfig)
    {
        await gatewayConfigManager.AddRouteConfigAsync(gatewayRouteConfig);
        return ResultModel.Success("添加成功");
    }

    /// <inheritdoc/>
    public async Task<ResultModel> EditRouteConfigAsync(GatewayRouteConfig gatewayRouteConfig)
    {
        await gatewayConfigManager.EditRouteConfigAsync(gatewayRouteConfig);
        return ResultModel.Success("修改成功");
    }

    /// <inheritdoc/>
    public async Task<ResultModel> DeleteRouteConfigAsync(string routeName)
    {
        await gatewayConfigManager.DeleteRouteConfigAsync(routeName);
        return ResultModel.Success("删除成功");
    }

    /// <inheritdoc/>
    public async Task<ResultModel<List<GatewayRouteConfig>>> GetAllRouteConfigsAsync()
    {
        List<GatewayRouteConfig> data = gatewayConfigManager.GetAllRouteConfigs();
        ResultModel<List<GatewayRouteConfig>> result = ResultModel<List<GatewayRouteConfig>>.Success(data, "查询成功");
        return await Task.FromResult(result);
    }
    /// <inheritdoc/>
    public async Task<ResultModel<GatewayRouteConfig>> GetRouteConfigAsync(string routeName)
    {
        GatewayRouteConfig data = gatewayConfigManager.GetRouteConfig(routeName);
        ResultModel<GatewayRouteConfig> result = ResultModel<GatewayRouteConfig>.Success(data, "查询成功");
        return await Task.FromResult(result);
    }
}
