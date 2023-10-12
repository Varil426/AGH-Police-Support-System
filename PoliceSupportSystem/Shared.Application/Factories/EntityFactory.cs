using Shared.Application.Agents.Communication.Messages;
using Shared.Application.Integration.DTOs;
using Shared.Domain.Incident;
using Shared.Domain.Patrol;

namespace Shared.Application.Factories;

internal class EntityFactory : IIncidentFactory, IPatrolFactory
{
    public Incident CreateIncident(NewIncidentDto newIncidentDto) => new(newIncidentDto.Id, newIncidentDto.Location, newIncidentDto.Status, newIncidentDto.Type);
    public Patrol CreatePatrol(NewPatrolDto newPatrolDto) => new(newPatrolDto.Id, newPatrolDto.PatrolId, newPatrolDto.Position, newPatrolDto.Status);

    public Patrol CreatePatrol(PatrolOnlineMessage patrolOnlineMessage) => new(
        patrolOnlineMessage.PatrolAgentId,
        patrolOnlineMessage.PatrolId,
        patrolOnlineMessage.Position,
        patrolOnlineMessage.Status);
}