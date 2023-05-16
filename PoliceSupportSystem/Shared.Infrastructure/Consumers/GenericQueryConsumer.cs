using MassTransit;
using Shared.Application;
using Shared.Application.Integration.Queries;

namespace Shared.Infrastructure.Consumers;

public sealed class GenericQueryConsumer<TQueryHandler, TQuery, TResult> : IConsumer<TQuery> where TQueryHandler : class, IQueryHandler<TQuery, TResult>
    where TQuery : class, IQuery<TResult>
    where TResult : class
{
    private readonly TQueryHandler _handler;

    public GenericQueryConsumer(TQueryHandler handler)
    {
        _handler = handler;
    }

    public async Task Consume(ConsumeContext<TQuery> context) => await context.RespondAsync(await _handler.Handle(context.Message));
}