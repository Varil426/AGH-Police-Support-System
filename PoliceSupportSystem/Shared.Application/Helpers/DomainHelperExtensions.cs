using Shared.Application.Integration.DTOs;
using Shared.Domain.Incident;

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
}