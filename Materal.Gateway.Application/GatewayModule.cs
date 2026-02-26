using Materal.Gateway.Extensions;

namespace Materal.Gateway.Application;

/// <summary>
/// 网关模块
/// </summary>
public partial class GatewayModule() : MergeBlockModule("网关模块")
{
    /// <summary>
    /// 配置服务
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override void OnConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddGateway(context.Configuration);
    }
    /// <summary>
    /// 应用程序初始化
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        AdvancedContext advancedContext = context.ServiceProvider.GetRequiredService<AdvancedContext>();
        if (advancedContext.App is not WebApplication app) return;
        app.UseGateway();
    }
}
