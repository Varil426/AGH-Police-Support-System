using Microsoft.Extensions.Hosting;
using Shared.Application.Services;

namespace Shared.Infrastructure.Tasks;

internal class AnnounceServiceOnlineOffline : BackgroundService
{
    private readonly IStatusService _statusService;

    public AnnounceServiceOnlineOffline(IStatusService statusService)
    {
        _statusService = statusService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _statusService.AnnounceOnline();

        try
        {
            while (!stoppingToken.IsCancellationRequested)
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
        finally
        {
            await _statusService.AnnounceOffline();
        }
    }
}