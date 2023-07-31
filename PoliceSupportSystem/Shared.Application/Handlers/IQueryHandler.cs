using Shared.Application.Integration.Queries;

namespace Shared.Application.Handlers;

public interface IQueryHandler<in TQuery, TResult> /*: IConsumer<TQuery>*/ where TQuery : /*class /* class - required by MassTransit #1#,*/ IQuery<TResult>
{
    public Task<TResult> Handle(TQuery query);
}