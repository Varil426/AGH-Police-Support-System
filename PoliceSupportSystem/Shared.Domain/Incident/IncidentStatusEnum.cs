namespace Shared.Domain.Incident;

public enum IncidentStatusEnum
{
    WaitingForResponse,
    OnGoingNormal,
    AwaitingBackup,
    OnGoingShooting,
    Resolved
}