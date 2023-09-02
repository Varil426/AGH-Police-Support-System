using Shared.Application.DTOs;
using Shared.Domain.Incident;

namespace Shared.Application.Helpers;

public static class DtoHelperExtensions
{
    public static IncidentDto AsDto(this Incident incident) => new IncidentDto(incident.Id, incident.Location, incident.Status, incident.Type);
}