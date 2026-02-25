using Consul;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace Materal.Gateway.ConfigProviders;

internal class ConsulIProxyConfigHostedService(ConsulIProxyConfigProvider provider) : IHostedService
{
    private ulong _lastIndex;

    [AllowNull]
    private CancellationTokenSource _cts;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _ = WatchServicesAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken) => await _cts.CancelAsync();

    /// <summary>
    /// 监听服务变化
    /// </summary>
    private async Task WatchServicesAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                QueryResult<Dictionary<string, string[]>> queryResult = await provider.ConsulClient.Catalog.Services(new QueryOptions
                {
                    WaitIndex = _lastIndex,
                    WaitTime = TimeSpan.FromSeconds(30)
                });

                if (queryResult.LastIndex != _lastIndex)
                {
                    _lastIndex = queryResult.LastIndex;
                    await provider.UpdateAsync();
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }
}
