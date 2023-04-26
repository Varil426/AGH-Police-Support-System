using Microsoft.Extensions.Hosting;

namespace Shared.Microservices;

public class MicroserviceBase : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}