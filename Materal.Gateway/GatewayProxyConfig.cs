using Microsoft.Extensions.Primitives;

namespace Materal.Gateway;

/// <summary>
/// 网关反向代理配置
/// </summary>
public class GatewayProxyConfig : IProxyConfig
{
    private readonly CancellationTokenSource _cts = new();
    /// <summary>
    /// 路由规则
    /// </summary>
    public IReadOnlyList<RouteConfig> Routes { get; }
    /// <summary>
    /// 集群映射
    /// </summary>
    public IReadOnlyList<ClusterConfig> Clusters { get; }
    /// <summary>
    /// 变更通知
    /// </summary>
    public IChangeToken ChangeToken { get; }
    /// <summary>
    /// 构造方法
    /// </summary>
    public GatewayProxyConfig() : this([], []) { }
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="routes"></param>
    /// <param name="clusters"></param>
    public GatewayProxyConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
    {
        ChangeToken = new CancellationChangeToken(_cts.Token);
        Routes = routes;
        Clusters = clusters;
    }
    /// <summary>
    /// 通知停止
    /// </summary>
    internal void SignalStop() => _cts.Cancel();
}
