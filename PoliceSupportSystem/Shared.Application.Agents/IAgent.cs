﻿using Microsoft.Extensions.Hosting;

namespace Shared.Application.Agents;

public interface IAgent : IHostedService
{
    Guid Id { get; }
    
    IEnumerable<Type> AcceptedMessageTypes { get; }
    
    IEnumerable<Type> AcceptedEnvironmentSignalTypes { get; }

    Task PushMessageAsync(IMessage message);
    Task PushMessagesAsync(IEnumerable<IMessage> messages);

    Task PushEnvironmentSignalAsync(IEnvironmentSignal signal);
    Task PushEnvironmentSignalsAsync(IEnumerable<IEnvironmentSignal> signals);

    Task RunAsync(CancellationToken cancellationToken) => RunAsync(cancellationToken, Array.Empty<string>());

    Task RunAsync(CancellationToken cancellationToken, params string[] args);

    Task KillAsync();
}