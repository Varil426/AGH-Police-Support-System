using Shared.CommonTypes.Geo;

namespace Shared.Application.Agents.Communication.Messages.PatrolOrders;

public record SupportShootingOrderMessage : BaseMessageWithAcknowledgeRequired
{
    public Guid IncidentId { get; }

    public Position IncidentLocation { get; }
    
    public SupportShootingOrderMessage(Guid sender, Guid MessageId, Guid receiver, Guid incidentId, Position incidentLocation, Guid? responseTo = null) : base(
        sender,
        MessageId,
        receiver,
        responseTo)
    {
        IncidentId = incidentId;
        IncidentLocation = incidentLocation;
    }
}