using MassTransit;
using MassTransit.Middleware;
using Shared.Application;
using Shared.Application.Integration.Queries;

namespace Shared.Infrastructure.Conventions;

internal class MethodQueryConsumerMessageFilter<TConsumer, TMessage, TResult> : IConsumerMessageFilter<TConsumer, TMessage>
    where TConsumer : class, IQueryHandler<TMessage, TResult>
    where TMessage : class, IQuery<TResult>
{
    public async Task Send(ConsumerConsumeContext<TConsumer, TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
    {
        if (context.Consumer is not IQueryHandler<TMessage, TResult> messageConsumer)
        {
            var message =
                $"Consumer type {TypeCache<TConsumer>.ShortName} is not a consumer of message type {TypeCache<TMessage>.ShortName}";

            throw new ConsumerMessageException(message);
        }

        await context.RespondAsync(await messageConsumer.Handle(context.Message) ?? throw new Exception("Something went wrong with Query handling.")); // TODO Change exception
    }

    public void Probe(ProbeContext context)
    {
        var scope = context.CreateScope("consume");
        scope.Add("method", $"Handle({TypeCache<TMessage>.ShortName} message)");
    }
}