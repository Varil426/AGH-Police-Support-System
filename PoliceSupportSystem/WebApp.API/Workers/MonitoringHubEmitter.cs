﻿using Microsoft.AspNetCore.SignalR;
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
            var incidents = _monitoringService.ActiveIncidents.Select(x => x.AsDto());
            var patrols = _monitoringService.Patrols.Select(x => x.AsDto());
            await _hubContext.Clients.All.ReceiveUpdate(new CityStateMessageDto(_monitoringService.HqLocation, incidents, patrols));
            // ReSharper disable once PossibleLossOfFraction
            await Task.Delay(TimeSpan.FromSeconds(1) / 20, stoppingToken);
        }
    }
}