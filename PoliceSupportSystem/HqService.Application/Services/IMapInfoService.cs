using Shared.CommonTypes.Geo;

namespace HqService.Application.Services;

public interface IMapInfoService
{
    Task<IList<string>> GetDistricts();
    Task<string> GetDistrict(Position position);
}