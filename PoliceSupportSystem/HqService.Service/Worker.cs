using Shared.Application.Agents.Communication.Messages;
using Wolverine;

namespace HqService.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMessageBus _messageBus;

    public Worker(ILogger<Worker> logger, IMessageBus messageBus)
    {
        _logger = logger;
        _messageBus = messageBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _messageBus.SendAsync(new TestMessage());
            await Task.Delay(1000, stoppingToken);
        }
    }
}

// TODO Remove