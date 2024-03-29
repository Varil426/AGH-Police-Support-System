﻿using Shared.CommonTypes.District;

namespace Simulation.Application.Entities;

public record District(string Name, DistrictDangerLevelEnum DangerLevel = DistrictDangerLevelEnum.Normal);