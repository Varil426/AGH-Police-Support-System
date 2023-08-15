﻿using Autofac;
using Simulation.Application.Directors;
using Simulation.Application.Directors.IncidentDirector;
using Simulation.Application.Services;

namespace Simulation.Application;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        
        builder.RegisterType<Simulation>().As<ISimulation>().SingleInstance();
        
        builder.RegisterType<IncidentDirector>().As<IDirector>().SingleInstance();
        
        builder.RegisterType<ServiceFactory>().As<IServiceFactory>();
        builder.RegisterType<EntityFactory>().As<IEntityFactory>();
        builder.RegisterType<IncidentRandomizer>().As<IIncidentRandomizer>();
    }
}