using Consul;
using System.Collections.Concurrent;
using DestinationConfig = Yarp.ReverseProxy.Configuration.DestinationConfig;

namespace Materal.Gateway.Consul;

/// <summary>
/// Consul网关主机
/// </summary>
public class ConsulGatewayHost : GatewayHost, IGatewayHost
{
    private readonly Timer _consulTimer;
    private int _consulInterval = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
    private IConsulClient _consulClient;
    private ConcurrentDictionary<string, List<string>> _consulServices = new();
    /// <summary>
    /// 构造方法
    /// </summary>
    public ConsulGatewayHost(IOptionsMonitor<GatewayConfig> config, GatewayProxyConfigProvider yarpConfigProvider, ILoggerFactory? loggerFactory = null) : base(config, yarpConfigProvider, loggerFactory)
    {
        _consulTimer = new(ConsulTimerCallback);
        if (config.CurrentValue.Consul is null) throw new GatewayException("获取Consul配置失败");
        _consulInterval = (int)TimeSpan.FromSeconds(config.CurrentValue.Consul.Interval).TotalMilliseconds;
        _consulClient = new ConsulClient(m =>
        {
            m.Address = new Uri(config.CurrentValue.Consul.Url);
            if (!string.IsNullOrWhiteSpace(config.CurrentValue.Consul.Token))
            {
                m.Token = config.CurrentValue.Consul.Token;
            }
        });
    }
    /// <inheritdoc/>
    public override async Task StartAsync()
    {
        await base.StartAsync();
        BeginConsulTimer(500);
    }
    /// <inheritdoc/>
    public override async Task StopAsync()
    {
        StopConsulTimer();
        await base.StopAsync();
    }
    /// <inheritdoc/>
    protected override void RefreshConfig()
    {
        if (Config.CurrentValue.Consul is not null)
        {
            _consulInterval = (int)TimeSpan.FromSeconds(Config.CurrentValue.Consul.Interval).TotalMilliseconds;
            _consulClient = new ConsulClient(m =>
            {
                m.Address = new Uri(Config.CurrentValue.Consul.Url);
                if (!string.IsNullOrWhiteSpace(Config.CurrentValue.Consul.Token))
                {
                    m.Token = Config.CurrentValue.Consul.Token;
                }
            });
        }
        base.RefreshConfig();
    }
    /// <inheritdoc/>
    protected override Dictionary<string, DestinationConfig> GetDestinationConfigs(GatewayRouteConfig config)
    {
        Dictionary<string, DestinationConfig> destinations = base.GetDestinationConfigs(config);
        if (config.Consul is null || string.IsNullOrWhiteSpace(config.Consul.ServiceName)) return destinations;
        ConcurrentDictionary<string, List<string>> nowConsulServices = _consulServices;
        if (!nowConsulServices.TryGetValue(config.Consul.ServiceName, out List<string>? address) || address is null || address.Count <= 0) return destinations;
        for (int i = 0; i < address.Count; i++)
        {
            DestinationConfig destination = new()
            {
                Address = $"{config.Consul.Schema}://{address[i]}",
            };
            destinations.Add($"{config.Name}ConsulDestination{i}", destination);
        }
        return destinations;
    }
    /// <summary>
    /// Consul定时器回调
    /// </summary>
    /// <param name="state"></param>
    private void ConsulTimerCallback(object? state)
    {
        try
        {
            Task task = FillConsulServicesAsync();
            task.Wait();
        }
        finally
        {
            BeginConsulTimer();
        }
    }
    /// <summary>
    /// 开始Consul定时器
    /// </summary>
    private void BeginConsulTimer(int? interval = null) => _consulTimer.Change(interval ?? _consulInterval, Timeout.Infinite);
    /// <summary>
    /// 停止Consul定时器
    /// </summary>
    private void StopConsulTimer()
    {
        _consulTimer.Change(Timeout.Infinite, Timeout.Infinite);
        _consulTimer.Dispose();
    }
    private async Task FillConsulServicesAsync()
    {
        IConsulClient nowConsulClient = _consulClient;
        QueryResult<Dictionary<string, AgentService>> consulResult = await nowConsulClient.Agent.Services();
        ConcurrentDictionary<string, List<string>> newConsulServices = [];
        foreach (KeyValuePair<string, AgentService> item in consulResult.Response)
        {
            string address = $"{item.Value.Address}:{item.Value.Port}";
            List<string> addressList;
            if (newConsulServices.TryGetValue(item.Value.Service, out List<string>? value))
            {
                addressList = value;
            }
            else
            {
                addressList = [];
                newConsulServices.TryAdd(item.Value.Service, addressList);
            }
            addressList.Add(address);
        }
        _consulServices = newConsulServices;
        RefreshYarpConfig();
    }
}
