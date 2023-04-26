using Shared.Agents;
using Shared.Agents.Communication.Messages;
using Shared.Agents.Communication.Signals;

namespace HqService.Application.Agents;

public class HqAgent : AgentBase
{
    private static readonly IReadOnlyCollection<Type> s_acceptedMessageTypes = new[] { typeof(TestMessage) }.AsReadOnly();
    private static readonly IReadOnlyCollection<Type> s_acceptedSignalTypes = new[] { typeof(TestSignal) }.AsReadOnly();


    public HqAgent(
        Guid id,
        IMessageService messageService) : base(id, s_acceptedMessageTypes, s_acceptedSignalTypes, messageService)
    {
    }

    protected override Task HandleMessage(IMessage message)
    {
        throw new NotImplementedException();
    }

    protected override Task HandleSignal(IEnvironmentSignal signal)
    {
        throw new NotImplementedException();
    }
}