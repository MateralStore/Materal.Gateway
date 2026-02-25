namespace Materal.Gateway.ConfigProviders;

/// <summary>
/// Consul配置选项
/// </summary>
public class ConsulOptions
{
    /// <summary>
    /// 主机地址
    /// </summary>
    public string Host { get; set; } = "http://127.0.0.1:8500";
}
