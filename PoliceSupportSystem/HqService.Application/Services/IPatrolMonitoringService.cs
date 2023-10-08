using Shared.Domain.Patrol;

namespace HqService.Application.Services;

internal interface IPatrolMonitoringService
{
    IReadOnlyCollection<Patrol> Patrols { get; }

    void AddPatrol(Patrol patrol);

    void RemovePatrol(Patrol patrol) => RemovePatrol(patrol.Id);

    void RemovePatrol(Guid id);
    void RemovePatrol(string patrolId);
}