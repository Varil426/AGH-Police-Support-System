using Autofac;
using Shared.Application.Helpers;

namespace PatrolService.Application;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterHostedService<PatrolAgent>();
    }
}