using Microsoft.Extensions.Hosting;

namespace Shared.Agents;

public interface IAgent : IHostedService
{
    Guid Id { get; }
    
    IEnumerable<string> AcceptedMessageTypes { get; }
    
    IEnumerable<string> AcceptedEnvironmentSignalTypes { get; }

    Task PushMessageAsync(IMessage message);

    Task PushEnvironmentSignalAsync(IEnvironmentSignal signal);

    Task RunAsync(CancellationToken cancellationToken) => RunAsync(cancellationToken, Array.Empty<string>());

    Task RunAsync(CancellationToken cancellationToken, params string[] args);

    Task KillAsync();
}