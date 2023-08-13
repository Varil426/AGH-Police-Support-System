﻿using Simulation.Application.Services;
using Simulation.Shared.Communication;

namespace Simulation.Application.Handlers;

public class ServiceOnlineMessageHandler : BaseSimulationMessageHandler<ServiceOnlineMessage>
{
    private readonly IServiceFactory _serviceFactory;

    public ServiceOnlineMessageHandler(IServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    public override Task HandleAsync(ISimulation simulation, ServiceOnlineMessage message)
    {
        simulation.AddService(_serviceFactory.CreateService(message.ServiceId, message.ServiceType));
        return Task.CompletedTask;
    }
}