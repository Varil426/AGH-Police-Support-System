
using MassTransit;

namespace Shared.Infrastructure.Filters;

public class QueryOutgoingFilter<T> : IFilter<SendContext<T>> where T : class
{
    public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
    {

        return Task.CompletedTask;
    }

    public void Probe(ProbeContext context)
    {
        var a = 123;
    }
}