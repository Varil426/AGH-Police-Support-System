using Reinforced.Typings.Attributes;

namespace Shared.CommonTypes.Incident;

[TsEnum]
public enum IncidentStatusEnum
{
    WaitingForResponse,
    OnGoingNormal,
    AwaitingBackup,
    OnGoingShooting,
    Resolved
}