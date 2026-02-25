namespace Materal.Gateway.Configs;

/// <summary>
/// 网关配置管理器
/// </summary>
public class GatewayConfigManager(IOptionsMonitor<GatewayConfig> options) : IGatewayConfigManager
{
    /// <summary>
    /// 配置文件路径
    /// </summary>
    internal static string ConfigFilePath { get; set; } = string.Empty;
    private List<GatewayRouteConfig> Routes => options.CurrentValue.Routes;
    private readonly static JsonSerializerOptions _jsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    /// <inheritdoc/>
    public async Task AddRouteConfigAsync(GatewayRouteConfig gatewayRouteConfig)
    {
        if (Routes.Any(m => m.Name == gatewayRouteConfig.Name)) throw new GatewayException("路由名称已存在");
        if (Routes.Any(m => m.UpstreamPathTemplate == gatewayRouteConfig.UpstreamPathTemplate)) throw new GatewayException("上游路径已存在");
        Routes.Add(gatewayRouteConfig);
        await SaveConfigAsync();
    }
    /// <inheritdoc/>
    public async Task EditRouteConfigAsync(GatewayRouteConfig gatewayRouteConfig)
    {
        if (Routes.Any(m => m.Name != gatewayRouteConfig.Name && m.UpstreamPathTemplate == gatewayRouteConfig.UpstreamPathTemplate)) throw new GatewayException("上游路径已存在");
        GatewayRouteConfig config = Routes.FirstOrDefault(m => m.Name == gatewayRouteConfig.Name) ?? throw new GatewayException("路由不存在");
        CopyPropertiesHelper.CopyProperties(gatewayRouteConfig, config);
        await SaveConfigAsync();
    }
    /// <inheritdoc/>
    public async Task DeleteRouteConfigAsync(string routeName)
    {
        GatewayRouteConfig config = Routes.FirstOrDefault(m => m.Name == routeName) ?? throw new GatewayException("路由不存在");
        Routes.Remove(config);
        await SaveConfigAsync();
    }
    /// <inheritdoc/>
    public List<GatewayRouteConfig> GetAllRouteConfigs() => Routes;
    /// <inheritdoc/>
    public GatewayRouteConfig GetRouteConfig(string routeName)
    {
        GatewayRouteConfig config = Routes.FirstOrDefault(m => m.Name == routeName) ?? throw new GatewayException("路由不存在");
        return config;
    }
    /// <summary>
    /// 保存配置
    /// </summary>
    /// <returns></returns>
    private async Task SaveConfigAsync()
    {
        FileInfo fileInfo = new(ConfigFilePath);
        string jsonContent = $"{{\"Gateway\":{options.CurrentValue.ToJson(_jsonSerializerOptions)}}}";
        await File.WriteAllTextAsync(fileInfo.FullName, jsonContent);
    }
}
