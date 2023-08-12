using Shared.Domain.Geo;
using Shared.Domain.Incident;

namespace HqService.Application.DTOs;

public record UpdateIncidentDto(
    Guid Id,
    IncidentTypeEnum NewIncidentType,
    IncidentTypeEnum OldIncidentType,
    IncidentStatusEnum NewIncidentStatus,
    IncidentStatusEnum OldIncidentStatus,
    Position? NewLocation = null);