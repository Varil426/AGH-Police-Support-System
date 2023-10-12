using Reinforced.Typings.Attributes;
using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Patrol;

namespace Shared.Application.Integration.DTOs;

[TsInterface]
public record PatrolDto(Guid Id, string PatrolId, Position Position, PatrolStatusEnum Status);