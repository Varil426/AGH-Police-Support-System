namespace Shared.Application.Integration.Commands;

public interface ICommand<out TResult> : IDirectMessage /*: ICommand*/
{
}

public interface ICommand : IDirectMessage
{
}
