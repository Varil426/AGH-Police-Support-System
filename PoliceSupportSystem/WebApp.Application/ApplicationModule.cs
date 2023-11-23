using Autofac;
using Microsoft.Extensions.Configuration;
using Shared.Application;
using Shared.Application.Services;
using WebApp.Application.Services;
using WebApp.Application.Services.Statistics;
using WebApp.Application.Settings;

namespace WebApp.Application;

public class ApplicationModule : ConfigurationAwareModule
{
    public ApplicationModule(IConfiguration configuration) : base(configuration)
    {
    }
    
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.Register(_ => GetSettings<MapSettings>(nameof(MapSettings))).As<MapSettings>().SingleInstance();

        builder.RegisterType<LocalDomainEventProcessor>().As<IDomainEventProcessor>();
        
        builder.RegisterType<CityStateMonitoringService>().As<ICityStateMonitoringService>().SingleInstance();
        builder.RegisterType<StatisticsManager>().As<IStatisticsManager>().SingleInstance();
    }

}