using Microsoft.Extensions.Hosting;

namespace Materal.Gateway;

/// <summary>
/// 网关主机服务
/// </summary>
public class GatewayHostedService(IGatewayHost gatewayToYarpAdapter) : IHostedService
{
    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken) => await gatewayToYarpAdapter.StartAsync();
    /// <inheritdoc/>
    public async Task StopAsync(CancellationToken cancellationToken) => await gatewayToYarpAdapter.StopAsync();
}
