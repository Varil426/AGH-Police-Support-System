﻿using Shared.Application.Services;
using Shared.Simulation.Services;

namespace Shared.Infrastructure.Settings;

public record PatrolSettings(string PatrolId) : IPatrolInfoService;