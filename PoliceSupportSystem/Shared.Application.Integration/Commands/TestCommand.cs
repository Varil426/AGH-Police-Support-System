namespace Shared.Application.Integration.Commands;

public record TestCommand(int Value, string Receiver) : ICommand<string>; // TODO Remove