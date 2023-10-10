using Riok.Mapperly.Abstractions;
using Shared.Application.Integration.DTOs;
using Shared.Domain.Incident;
using Shared.Domain.Patrol;

namespace Shared.Application.Helpers;

[Mapper]
public static partial class DtoHelperExtensions
{
    public static IncidentDto AsDto(this Incident incident) => new IncidentDto(incident.Id, incident.Location, incident.Status, incident.Type);

    public static partial PatrolDto AsDto(this Patrol patrol);
}