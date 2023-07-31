using Shared.Application.Common;
using Shared.Application.Services;

namespace Shared.Infrastructure.Settings;

public record ServiceSettings(string Id, ServiceTypeEnum ServiceType) : IServiceInfoService;