using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;

namespace Shared.Application.Integration.DTOs;

public record NewIncidentDto(Guid Id, Position Location, IncidentTypeEnum Type = IncidentTypeEnum.NormalIncident, IncidentStatusEnum Status = IncidentStatusEnum.WaitingForResponse);