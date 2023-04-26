namespace Shared.Agents;

public interface ISignalService
{
    internal Task SubscribeForSignalsAsync(IAgent agent);
}