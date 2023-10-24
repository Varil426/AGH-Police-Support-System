using Shared.CommonTypes.Geo;

namespace NavigationService.Application.Services;

public interface INavigationService
{
    public Task<Position> GetCurrentPosition();

    public Task DisplayDistrict(string districtName);
    
    public Task DisplayRouteTo(Position position);
}