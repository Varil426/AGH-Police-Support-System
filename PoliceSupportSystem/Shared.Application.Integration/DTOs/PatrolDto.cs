using Reinforced.Typings.Attributes;
using Shared.CommonTypes.Geo;

namespace Shared.Application.Integration.DTOs;

[TsInterface]
public record PatrolDto(Guid Id, string PatrolId, Position Position);