using Autofac;
using NavigationService.Application.Services;
using NavigationService.Simulation.Services;

namespace NavigationService.Simulation;

public class SimulationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<SimulationNavigationService>().AsSelf().As<INavigationService>().SingleInstance();
    }
}