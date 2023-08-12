using Shared.Domain.Geo;
using Shared.Domain.Incident;

namespace HqService.Application.DTOs;

public record NewIncidentDto(Guid Id, Position Location, IncidentTypeEnum Type = IncidentTypeEnum.NormalIncident, IncidentStatusEnum Status = IncidentStatusEnum.WaitingForResponse);