using Shared.Application.Services;

namespace HqService.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMessageBus _messageBus;
    private readonly IStatusService _statusService;

    public Worker(ILogger<Worker> logger, IMessageBus messageBus, IStatusService statusService)
    {
        _logger = logger;
        _messageBus = messageBus;
        _statusService = statusService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // _logger.LogInformation("TEST1");
            // await _messageBus.SendAsync(new TestMessage { Receivers = new [] { Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")  }});
            // await _messageBus.PublishAsync(new TestEvent("TEST", DateTimeOffset.UtcNow));
            // var commandResult = await _messageBus.InvokeAsync<TestCommand, string>(new TestCommand(123, "hq-service"));
            // var queryResult = await _messageBus.QueryAsync<TestQuery, string>(new TestQuery(10, "hq-service"));
            // Console.WriteLine($"Worker: {queryResult}");
            // Console.WriteLine($"Worker: {commandResult}");
            await Task.Delay(1000, stoppingToken);

            // await _statusService.AnnounceOnline();
        }
    }
}

// TODO Remove