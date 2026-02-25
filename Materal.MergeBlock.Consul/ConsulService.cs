using Consul;
using Materal.MergeBlock.Web.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Materal.MergeBlock.Consul;

/// <summary>
/// Consul服务
/// </summary>
public class ConsulService(IOptionsMonitor<ConsulOptions> options, ConsulServiceData data, ListeningUris uris) : IHostedService
{
    private readonly ConsulClient _consulClient = new(config => config.Address = new Uri(options.CurrentValue.Host));
    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!options.CurrentValue.Enable) return;
        AgentServiceRegistration registration = GetAgentServiceRegistration();
        await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _consulClient.Agent.ServiceDeregister(data.ServiceIDText, cancellationToken);
        _consulClient.Dispose();
    }

    private AgentServiceRegistration GetAgentServiceRegistration()
    {
        #region 服务路径Uri
        Uri serviceUrl;
        if (string.IsNullOrWhiteSpace(options.CurrentValue.ServiceHost))
        {
            serviceUrl = uris.First();
        }
        else
        {
            serviceUrl = new(options.CurrentValue.ServiceHost);
        }
        #endregion
        #region 健康检查Uri
        Uri healthUri;
        if (string.IsNullOrWhiteSpace(options.CurrentValue.HealthHost))
        {
            healthUri = serviceUrl;
        }
        else
        {
            healthUri = new(options.CurrentValue.HealthHost);
        }
        healthUri = new(healthUri, $"/api/Health?id={data.ServiceIDText}");
        #endregion

        return new()
        {
            ID = data.ServiceIDText,
            Name = options.CurrentValue.Name,
            Address = serviceUrl.Host,
            Port = serviceUrl.Port,
            Tags = [.. options.CurrentValue.Tags],
            Check = new AgentServiceCheck
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                HTTP = healthUri.AbsoluteUri,
                TLSSkipVerify = healthUri.Scheme == "https",
                Interval = TimeSpan.FromSeconds(options.CurrentValue.HealthInterval),
                Timeout = TimeSpan.FromSeconds(5),
            }
        };
    }
}
