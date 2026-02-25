using Materal.Gateway.Consul.Extensions;
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
        context.Services.AddConsulGateway(option =>
        {
            option.Configuration = context.Configuration;
            string rootPath = typeof(GatewayModule).Assembly.GetDirectoryPath() ?? AppDomain.CurrentDomain.BaseDirectory;
            option.ConfigFilePath = Path.Combine(rootPath, "GatewayConfig.json");
        });
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
