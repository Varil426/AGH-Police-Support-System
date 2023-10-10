using Shared.Application.Agents.Communication.Messages;
using Shared.Application.Integration.DTOs;
using Shared.Domain.Patrol;

namespace Shared.Application.Factories;

public interface IPatrolFactory
{
    public Patrol CreatePatrol(NewPatrolDto newPatrolDto);

    public Patrol CreatePatrol(PatrolOnlineMessage patrolOnlineMessage);
}