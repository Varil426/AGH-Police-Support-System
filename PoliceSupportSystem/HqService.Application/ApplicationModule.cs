using Autofac;
using HqService.Application.Agents;
using HqService.Application.Services;
using Shared.Application.Helpers;

namespace HqService.Application;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<ReportingService>().As<IReportingService>();
        builder.RegisterType<IncidentMonitoringService>().As<IIncidentMonitoringService>().SingleInstance();
        builder.RegisterType<PatrolMonitoringService>().As<IPatrolMonitoringService>().SingleInstance();

        builder.RegisterHostedService<HqAgent>();
    }
}