using Shared.CommonTypes.Patrol;
using Shared.Domain.Patrol;
using Simulation.Application.Entities.Patrol.Orders;
using Simulation.Communication.Common;
using Action = Simulation.Application.Entities.Patrol.Actions.Action;

namespace Simulation.Application.Entities.Patrol;

public interface ISimulationPatrol : IPatrol
{
    DateTimeOffset ActionChangedAt { get; }
    DateTimeOffset OrderReceivedAt { get; }
    Action Action { get; set; }
    Order? Order { get; set; }
    IReadOnlyCollection<IService> RelatedServices { get; }
    void AddRelatedService(IService service);
    void RemoveRelatedService(IService service);
    void RemoveRelatedService(string relatedServiceId);
    IEnumerable<IService> GetRelatedServicesOfType(ServiceTypeEnum serviceType);

    bool IsInEmergencyState => Status is PatrolStatusEnum.InShooting or PatrolStatusEnum.ResolvingIncident;
}