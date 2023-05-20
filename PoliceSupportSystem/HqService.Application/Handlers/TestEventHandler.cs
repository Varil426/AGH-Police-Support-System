using Shared.Application;
using Shared.Application.Integration.Events;

namespace HqService.Application.Handlers;

public class TestEventHandler : IEventHandler<TestEvent>
{
    public Task Handle(TestEvent @event)
    {
        Console.WriteLine($"From handler: {@event.Value}");
        return Task.CompletedTask;
    }
}