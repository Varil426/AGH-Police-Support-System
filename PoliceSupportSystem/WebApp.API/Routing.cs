using System.Net;
using WebApp.API.Handlers;
using WebApp.API.Hubs.MonitoringHub;

namespace WebApp.API;

public static class Routing
{
    private const string ApiPrefix = "/api";
    private const string MonitoringRoute = $"{ApiPrefix}/monitoring";
    private const string StatisticsRoute = $"{ApiPrefix}/statistics";
    private const string HealthCheckRoute = $"{ApiPrefix}/health-check";
    
    public static WebApplication AddRouting(this WebApplication app)
    {
        app.MapHub<MonitoringHub>(MonitoringRoute);
        app.MapGet(StatisticsRoute, (GetStatisticsHandler handler) => handler.Handle());
        app.MapGet(HealthCheckRoute, () => HttpStatusCode.OK);
        return app;
    }
}