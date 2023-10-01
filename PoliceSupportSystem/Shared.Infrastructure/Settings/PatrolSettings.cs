using Shared.Application.Services;

namespace Shared.Infrastructure.Settings;

public record PatrolSettings(string PatrolId) : IPatrolInfoService;