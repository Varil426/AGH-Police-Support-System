namespace Shared.Application.Agents.Communication.Messages.PatrolOrders;

public record PatrolDistrictOrderMessage : BaseMessageWithAcknowledgeRequired
{
    public string DistrictName { get; }

    public PatrolDistrictOrderMessage(Guid sender, Guid MessageId, Guid receiver, string districtName, Guid? responseTo = null) : base(sender, MessageId, receiver, responseTo)
    {
        DistrictName = districtName;
    }
}