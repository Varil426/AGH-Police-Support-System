using Shared.Application.Integration.DTOs;
using Shared.Domain.Incident;
using Shared.Domain.Patrol;

namespace Shared.Application.Helpers;

public static class DomainHelperExtensions
{
    public static void Update(this Incident incident, UpdateIncidentDto dto)
    {
        if (dto.NewIncidentType != incident.Type)
            incident.UpdateType(dto.NewIncidentType);
        
        if (dto.NewIncidentStatus != incident.Status)
            incident.UpdateStatus(dto.NewIncidentStatus);
        
        if (dto.NewLocation != incident.Location && dto.NewLocation is not null)
            incident.UpdateLocation(dto.NewLocation);
    }

    public static void Update(this Patrol patrol, PatrolDto dto)
    {
        patrol.UpdatePosition(dto.Position);
        patrol.UpdateStatus(dto.Status);
    }
}