namespace Shared.Application.Integration.Commands;

public interface ICommand<out TResult> : IDirectMessage /*: ICommand*/
{
    // string Receiver { get; init; }
}

// public interface IDirectCommand<out TResult> : ICommand<TResult>
// {
//     string Receiver { get; init; }
// }

public interface ICommand : IDirectMessage
{
    // string Receiver { get; init; }
}

// public interface IDirectCommand : ICommand
// {
//     string Receiver { get; init; }
// }