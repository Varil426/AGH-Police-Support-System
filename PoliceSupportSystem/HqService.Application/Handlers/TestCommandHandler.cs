using Shared.Application;
using Shared.Application.Handlers;
using Shared.Application.Integration.Commands;

namespace HqService.Application.Handlers; // TODO Remove whole file

public class TestCommandHandler : ICommandHandler<TestCommand, string>
{
    public Task<string> Handle(TestCommand command)
    {
        Console.WriteLine("TEST");
        return Task.FromResult("TEST SUCCESSFUL");
    }
}