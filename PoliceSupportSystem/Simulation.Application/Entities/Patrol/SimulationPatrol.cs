using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Patrol;
using Simulation.Application.DomainEvents;
using Simulation.Application.Entities.Patrol.Actions;
using Simulation.Application.Entities.Patrol.Orders;
using Simulation.Communication.Common;
using Action = Simulation.Application.Entities.Patrol.Actions.Action;

namespace Simulation.Application.Entities.Patrol;

public class SimulationPatrol : Shared.Domain.Patrol.Patrol, ISimulationRootEntity, ISimulationPatrol
{
    private readonly List<IService> _relatedServices = new();
    
    private Action _action;
    
    private Order? _order;
    
    public DateTimeOffset ActionChangedAt { get; private set; }
    public DateTimeOffset OrderReceivedAt { get; private set; }

    public Action Action
    {
        get => _action;
        set
        {
            if (_action == value)
                return;
            _action = value;
            var instance = DateTimeOffset.UtcNow;
            UpdateUpdatedAt(instance);
            ActionChangedAt = instance;
            AddDomainEvent(new PatrolActionChanged(this, _action));
        }
    }
    
    public Order? Order
    {
        get => _order;
        set
        {
            if (_order == value)
                return;
            _order = value;
            var instance = DateTimeOffset.UtcNow;
            UpdateUpdatedAt(instance);
            OrderReceivedAt = instance;
            AddDomainEvent(new PatrolOrderChanged(this, _order));
        }
    }

    public IReadOnlyCollection<IService> RelatedServices => _relatedServices.AsReadOnly();

    public SimulationPatrol(Guid id, string patrolId, Position position, PatrolStatusEnum status) : base(id, patrolId, position, status)
    {
        _action = new WaitingAction();
    }

    public void AddRelatedService(IService service)
    {
        _relatedServices.Add(service);
        UpdateUpdatedAt();
        AddDomainEvent(new PatrolRelatedServiceAdded(this, service));
    }

    public void RemoveRelatedService(IService service) => RemoveRelatedService(service.Id);

    public void RemoveRelatedService(string relatedServiceId) => 
        _relatedServices.Remove(_relatedServices.First(x => x.Id.Equals(relatedServiceId, StringComparison.InvariantCultureIgnoreCase)));

    public IEnumerable<IService> GetRelatedServicesOfType(ServiceTypeEnum serviceType) => RelatedServices.Where(x => x.ServiceType == serviceType);
}