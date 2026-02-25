namespace Materal.Gateway.ConfigProviders;

/// <summary>
/// Consul代理配置供应者
/// </summary>
public class ConsulIProxyConfigProvider : IProxyConfigProvider
{
    private readonly GatewayProxyConfig config = new();
    /// <inheritdoc/>
    public IProxyConfig GetConfig() => config;
}
