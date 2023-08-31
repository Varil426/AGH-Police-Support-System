using Shared.CommonTypes.Geo;

namespace Simulation.Application.Services;

public interface IMapService
{
    Task<IEnumerable<string>> GetDistrictNames();

    Task<IEnumerable<Position>> GetRandomPositionsInDistrict(string districtName, int numberOfPositions = 1);

    async Task<Position?> GetRandomPositionInDistrict(string districtName) => (await GetRandomPositionsInDistrict(districtName)).FirstOrDefault();
}