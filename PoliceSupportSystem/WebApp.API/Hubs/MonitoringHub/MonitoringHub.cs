using Microsoft.AspNetCore.SignalR;
using Shared.Application.Helpers;
using WebApp.Application.Services;

namespace WebApp.API.Hubs.MonitoringHub;

public class MonitoringHub : Hub<IMonitoringHubClient>
{
    public MonitoringHub(ICityStateMonitoringService monitoringService)
    {
        monitoringService.Subscribe(CityStateChanged);
    }

    // TODO Add an emitter (background worker)
    private async Task CityStateChanged(ICityStateMonitoringService monitoringService)
    {
        var incidents = monitoringService.GetIncidents().Select(x => x.AsDto());
        await Clients.All.ReceiveUpdate(new CityStateMessage(incidents));
    }
}