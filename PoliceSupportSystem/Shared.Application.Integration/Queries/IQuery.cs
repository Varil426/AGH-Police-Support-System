namespace Shared.Application.Integration.Queries;

public interface IQuery<out TResult> : IDirectMessage//: IQuery
{
    // string Receiver { get; init; }
}

// public interface IDirectQuery<out TResult> : IQuery<TResult>
// {
//     string Receiver { get; init; }
// }

// public interface IQuery
// {
//     
// }