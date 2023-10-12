using Autofac;
using HqService.Application.Services;
using HqService.Simulation.Services;

namespace HqService.Simulation;

public class SimulationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<SimulationMapInfoService>().As<IMapInfoService>().SingleInstance();
    }
}