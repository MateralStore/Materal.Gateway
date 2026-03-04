namespace Materal.MergeBlock.Consul;

/// <summary>
/// Consul配置
/// </summary>
[Options("Consul")]
public class ConsulOptions : IOptions
{
    /// <summary>
    /// 启用标识
    /// </summary>
    public bool Enable { get; set; } = false;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = "MergeBlockAPI";

    /// <summary>
    /// 主机
    /// </summary>
    public string Host { get; set; } = "http://127.0.0.1:8500";

    /// <summary>
    /// 标签
    /// </summary>
    public string[] Tags { get; set; } = ["Materal", "Materal.MergeBlock"];

    /// <summary>
    /// 健康检查间隔(秒)
    /// </summary>
    public int HealthInterval { get; set; } = 10;

    /// <summary>
    /// 健康检查主机
    /// </summary>
    public string? HealthHost { get; set; }

    /// <summary>
    /// 服务主机
    /// </summary>
    public string? ServiceHost { get; set; }

    /// <summary>
    /// 元数据
    /// </summary>
    public List<MetaData> MetaData { get; set; } = [];
}
