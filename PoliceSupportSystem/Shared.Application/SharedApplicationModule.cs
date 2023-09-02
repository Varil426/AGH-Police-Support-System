using Autofac;
using Shared.Application.Factories;
using Shared.Application.Services;

namespace Shared.Application;

public class SharedApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<DomainEventMapper>().As<IDomainEventMapper>();
        builder.RegisterType<DomainEventProcessor>().As<IDomainEventProcessor>();
        builder.RegisterType<EntityFactory>().As<IIncidentFactory>();
    }
}