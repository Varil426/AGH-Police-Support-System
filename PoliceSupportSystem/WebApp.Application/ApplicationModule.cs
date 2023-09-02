using Autofac;
using WebApp.Application.Services;

namespace WebApp.Application;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<CityStateMonitoringService>().As<ICityStateMonitoringService>().SingleInstance();
    }
}