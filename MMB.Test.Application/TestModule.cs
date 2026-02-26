using MMB.Test.Application.Hubs;

namespace Materal.Gateway.Application;

/// <summary>
/// 测试模块
/// </summary>
public partial class TestModule() : MergeBlockModule("网关模块")
{
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        base.OnApplicationInitialization(context);
        AdvancedContext advancedContext = context.ServiceProvider.GetRequiredService<AdvancedContext>();
        if (advancedContext.App is not WebApplication webApplication) return;
        webApplication.MapHub<TestHub>("/hubs/test");
    }
}
