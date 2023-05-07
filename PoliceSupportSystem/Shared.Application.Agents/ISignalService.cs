namespace Shared.Application.Agents;

public interface ISignalService
{
    internal Task SubscribeForSignalsAsync(IAgent agent);
}