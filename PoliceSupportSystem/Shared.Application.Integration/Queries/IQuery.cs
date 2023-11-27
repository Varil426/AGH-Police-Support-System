namespace Shared.Application.Integration.Queries;

public interface IQuery<out TResult> : IDirectMessage
{
}