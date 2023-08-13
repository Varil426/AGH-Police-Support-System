using Simulation.Application.Entities;

namespace Simulation.Application.Services;

public interface IMapService
{
    Task<IEnumerable<string>> GetDistrictNames();
}