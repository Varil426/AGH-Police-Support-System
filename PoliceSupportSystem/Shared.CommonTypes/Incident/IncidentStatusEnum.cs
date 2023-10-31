using Reinforced.Typings.Attributes;

namespace Shared.CommonTypes.Incident;

[TsEnum]
public enum IncidentStatusEnum
{
    WaitingForResponse,
    AwaitingPatrolArrival,
    OnGoingNormal,
    OnGoingShooting,
    Resolved
}