namespace Shared.Application.Agents.Communication.Messages;

public record ShowDistrictMessage(Guid Sender, Guid Receiver, string DistrictName) : BaseMessageWithAcknowledgeRequired(Sender, Guid.NewGuid(), Receiver);