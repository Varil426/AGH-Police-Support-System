namespace Shared.Application.Integration.Queries;

public record TestQuery(int Value, string Receiver) : IQuery<string>; // TODO Remove