using Autofac;
using Microsoft.Extensions.Hosting;
using Simulation.Application.Directors;
using Simulation.Application.Directors.IncidentDirector;
using Simulation.Application.Directors.PatrolDirector;
using Simulation.Application.Services;

namespace Simulation.Application;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        
        builder.RegisterType<Simulation>().As<ISimulation>().SingleInstance();
        builder.RegisterType<SimulationTimeService>().As<ISimulationTimeService>().SingleInstance();
        
        builder.RegisterType<IncidentDirector>().As<IDirector>().As<IHostedService>().SingleInstance();
        builder.RegisterType<PatrolDirector>().As<IDirector>().SingleInstance();
        
        builder.RegisterType<ServiceFactory>().As<IServiceFactory>();
        builder.RegisterType<EntityFactory>().As<ISimulationIncidentFactory>();
        builder.RegisterType<EntityFactory>().As<ISimulationPatrolFactory>();
        builder.RegisterType<IncidentRandomizer>().As<IIncidentRandomizer>();
        builder.RegisterType<DomainEventProcessor>().As<IDomainEventProcessor>();
        builder.RegisterType<DomainEventMapper>().As<IDomainEventMapper>();
        builder.RegisterType<RouteBuilder>().As<IRouteBuilder>();
    }
}