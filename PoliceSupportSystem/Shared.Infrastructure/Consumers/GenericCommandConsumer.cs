using MassTransit;
using Shared.Application;
using Shared.Application.Integration.Commands;

namespace Shared.Infrastructure.Consumers;

public sealed class GenericCommandConsumer<TCommand, TResult> : IConsumer<TCommand> where TCommand : class, ICommand<TResult> where TResult : class
{
    private readonly ICommandHandler<TCommand, TResult> _handler;

    public GenericCommandConsumer(ICommandHandler<TCommand, TResult> handler)
    {
        _handler = handler;
    }

    public async Task Consume(ConsumeContext<TCommand> context) => await context.RespondAsync(await _handler.Handle(context.Message));
}

public sealed class GenericCommandConsumer<TCommand> : IConsumer<TCommand> where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> _handler;

    public GenericCommandConsumer(ICommandHandler<TCommand> handler)
    {
        _handler = handler;
    }

    public Task Consume(ConsumeContext<TCommand> context) => _handler.Handle(context.Message);
}