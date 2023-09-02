using Reinforced.Typings.Attributes;
using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;

namespace Shared.Application.DTOs;

[TsInterface]
public record IncidentDto(Guid Id, Position Location, IncidentStatusEnum Status, IncidentTypeEnum Type);