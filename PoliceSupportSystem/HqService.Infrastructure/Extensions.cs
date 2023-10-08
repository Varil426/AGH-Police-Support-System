using HqService.Application.Services;
using HqService.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Infrastructure;

namespace HqService.Infrastructure;

public static class Extensions
{
    public static IHostBuilder AddHqAgentSettings(this IHostBuilder builder)
    {
        builder.ConfigureServices(
            (context, services) =>
            {
                var hqAgentSettings = context.Configuration.GetSettings<HqAgentSettings>(nameof(HqAgentSettings));
                services.AddSingleton(hqAgentSettings);
                services.AddSingleton<IHqInfoService>(hqAgentSettings);
            });
    
        return builder;
    }
}