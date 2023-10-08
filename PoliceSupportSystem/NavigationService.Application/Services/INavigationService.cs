﻿using Shared.CommonTypes.Geo;

namespace NavigationService.Application.Services;

public interface INavigationService
{
    public Task<Position> GetCurrentPosition();
}