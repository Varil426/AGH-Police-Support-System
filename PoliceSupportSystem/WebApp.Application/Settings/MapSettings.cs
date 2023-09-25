using Shared.CommonTypes.Geo;

namespace WebApp.Application.Settings;

internal record MapSettings()
{
    public required Position HqLocation { get; init; }
}