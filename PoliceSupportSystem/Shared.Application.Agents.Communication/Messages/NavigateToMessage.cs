using Shared.CommonTypes.Geo;

namespace Shared.Application.Agents.Communication.Messages;

public record NavigateToMessage(Guid Sender, Guid Receiver, Position Position, bool IsEmergency = false) : BaseMessageWithAcknowledgeRequired(Sender, Guid.NewGuid(), Receiver);