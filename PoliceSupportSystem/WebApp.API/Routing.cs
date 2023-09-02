using WebApp.API.Hubs;
using WebApp.API.Hubs.MonitoringHub;

namespace WebApp.API;

public static class Routing
{
    private const string ApiPrefix = "/api";
    private const string MonitoringRoute = $"{ApiPrefix}/monitoring";
    
    public static WebApplication AddRouting(this WebApplication app)
    {
        app.MapHub<MonitoringHub>(MonitoringRoute);
        return app;
    }
}