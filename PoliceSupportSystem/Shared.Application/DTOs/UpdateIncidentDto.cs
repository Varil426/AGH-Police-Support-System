using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;

namespace Shared.Application.DTOs;

public record UpdateIncidentDto(
    Guid Id,
    IncidentTypeEnum NewIncidentType,
    IncidentTypeEnum OldIncidentType,
    IncidentStatusEnum NewIncidentStatus,
    IncidentStatusEnum OldIncidentStatus,
    Position? NewLocation = null);