using Autofac;
using Shared.Application.Helpers;

namespace NavigationService.Application;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterAgent<NavigationAgent>();
    }
}