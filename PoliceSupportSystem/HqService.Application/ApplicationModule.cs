using Autofac;
using HqService.Application.Agents;
using HqService.Application.Services;
using HqService.Application.Services.DecisionService;
using HqService.Application.Settings;
using Microsoft.Extensions.Configuration;
using Shared.Application;
using Shared.Application.Helpers;

namespace HqService.Application;

public class ApplicationModule : ConfigurationAwareModule
{
    public ApplicationModule(IConfiguration configuration) : base(configuration)
    {
    }
    
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<ReportingService>().As<IReportingService>();
        builder.RegisterType<IncidentMonitoringService>().As<IIncidentMonitoringService>().SingleInstance();
        builder.RegisterType<PatrolMonitoringService>().As<IPatrolMonitoringService>().SingleInstance();
        builder.RegisterType<DecisionService>().As<IDecisionService>();
        builder.Register(_ => GetSettings<DecisionServiceSettings>(nameof(DecisionServiceSettings))).As<DecisionServiceSettings>().SingleInstance();
        
        builder.RegisterAgent<HqAgent>();
    }
}