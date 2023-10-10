using Shared.CommonTypes.Geo;

namespace Shared.Application.Integration.DTOs;

public record NewPatrolDto(Guid Id, string PatrolId, Position Position);