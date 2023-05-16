using Shared.Application.Integration.Commands;

namespace Shared.Application;

public interface ICommandHandler<in TCommand> /*: IConsumer<TCommand>*/ where TCommand : /*class /* class - required by MassTransit #1#,*/ ICommand
{
    Task Handle(TCommand command);
}

public interface ICommandHandler<in TCommand, TResult> /*: IConsumer<TCommand>*/ /*: ICommandHandler<TCommand>*/
    where TCommand : /*class /* class - required by MassTransit #1#,*/ ICommand<TResult>
{
    /*new*/
    Task<TResult> Handle(TCommand command);
}