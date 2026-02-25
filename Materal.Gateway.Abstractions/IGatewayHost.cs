namespace Materal.Gateway.Abstractions;

/// <summary>
/// 网关主机
/// </summary>
public interface IGatewayHost
{
    /// <summary>
    /// 启动
    /// </summary>
    /// <returns></returns>
    Task StartAsync();
    /// <summary>
    /// 停止
    /// </summary>
    /// <returns></returns>
    Task StopAsync();
}
