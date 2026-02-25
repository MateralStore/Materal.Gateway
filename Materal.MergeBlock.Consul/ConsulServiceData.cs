using Materal.Extensions.DependencyInjection;

namespace Materal.MergeBlock.Consul;

/// <summary>
/// Consul服务数据
/// </summary>
public class ConsulServiceData : ISingletonDependency<ConsulServiceData>
{
    /// <summary>
    /// 服务ID
    /// </summary>
    public Guid ServiceID { get; } = Guid.NewGuid();

    /// <summary>
    /// 服务ID文本
    /// </summary>
    public string ServiceIDText => ServiceID.ToString();

    /// <summary>
    /// 验证服务
    /// </summary>
    /// <param name="serviceID"></param>
    /// <returns></returns>
    public bool VerificationService(Guid serviceID) => ServiceID.Equals(serviceID);
}