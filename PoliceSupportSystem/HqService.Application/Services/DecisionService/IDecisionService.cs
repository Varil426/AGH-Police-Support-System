using Shared.Domain.Incident;
using Shared.Domain.Patrol;

namespace HqService.Application.Services.DecisionService;

public interface IDecisionService
{
    Task<IReadOnlyList<PatrolOrder>> ComputeOrders(IEnumerable<Incident> onGoingIncidents, IEnumerable<Patrol> patrols);
}