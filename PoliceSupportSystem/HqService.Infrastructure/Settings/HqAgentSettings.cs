using HqService.Application.Services;

namespace HqService.Infrastructure.Settings;

public record HqAgentSettings(Guid HqAgentId) : IHqInfoService;