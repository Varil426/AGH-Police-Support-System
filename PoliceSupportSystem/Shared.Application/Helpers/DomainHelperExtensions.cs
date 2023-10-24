using Shared.Application.Integration.DTOs;
using Shared.Domain.Incident;
using Shared.Domain.Patrol;

namespace Shared.Application.Helpers;

public static class DomainHelperExtensions
{
    public static void Update(this Incident incident, IncidentDto dto)
    {
        if (dto.Type != incident.Type)
            incident.UpdateType(dto.Type);
        
        if (dto.Status != incident.Status)
            incident.UpdateStatus(dto.Status);
        
        if (dto.Location != incident.Location)
            incident.UpdateLocation(dto.Location);
    }

    public static void Update(this Patrol patrol, PatrolDto dto)
    {
        patrol.UpdatePosition(dto.Position);
        patrol.UpdateStatus(dto.Status);
    }
}