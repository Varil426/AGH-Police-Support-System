using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Patrol;

namespace Shared.Application.Integration.DTOs;

public record NewPatrolDto(Guid Id, string PatrolId, Position Position, PatrolStatusEnum Status);