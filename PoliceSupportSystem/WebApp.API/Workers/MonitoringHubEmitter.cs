using Microsoft.AspNetCore.SignalR;
using Shared.Application.Helpers;
using WebApp.API.Hubs.MonitoringHub;
using WebApp.Application.Services;

namespace WebApp.API.Workers;

public class MonitoringHubEmitter : BackgroundService
{
    private readonly IHubContext<MonitoringHub, IMonitoringHubClient> _hubContext;
    private readonly ICityStateMonitoringService _monitoringService;

    public MonitoringHubEmitter(IHubContext<MonitoringHub, IMonitoringHubClient> hubContext, ICityStateMonitoringService monitoringService)
    {
        _hubContext = hubContext;
        _monitoringService = monitoringService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var incidents = _monitoringService.GetIncidents().Select(x => x.AsDto());
            await _hubContext.Clients.All.ReceiveUpdate(new CityStateMessage(incidents));
            // ReSharper disable once PossibleLossOfFraction
            await Task.Delay(TimeSpan.FromMilliseconds(1000 / 60), stoppingToken);
        }
    }
}