using Shared.CommonTypes.Geo;

namespace Shared.Application.Agents.Communication.Messages;

public record NavigateToMessage(Guid Sender, Guid Receiver, Position Position) : BaseMessageWithAcknowledgeRequired(Sender, Guid.NewGuid(), Receiver);