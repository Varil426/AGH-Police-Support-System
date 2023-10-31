using Autofac;
using PatrolService.Application.Services;
using PatrolService.Simulation.Services;

namespace PatrolService.Simulation;

public class SimulationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<SimulationConfirmationService>().As<IConfirmationService>();
    }
}