using Materal.MergeBlock.Abstractions.Extensions;

namespace Materal.MergeBlock.Consul;

/// <summary>
/// Consul模块
/// </summary>
public class ConsulModule() : MergeBlockModule("服务发现模块")
{
    /// <inheritdoc/>
    public override void OnConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMergeBlockHostedService<ConsulService>();
    }
}
