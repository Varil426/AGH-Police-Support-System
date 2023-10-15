using Reinforced.Typings.Attributes;

namespace Shared.CommonTypes.Patrol;

[TsEnum]
public enum PatrolStatusEnum
{
    AwaitingOrders,
    Patrolling,
    ResolvingIncident,
    InShooting
}