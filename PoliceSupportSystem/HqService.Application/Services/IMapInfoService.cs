namespace HqService.Application.Services;

public interface IMapInfoService
{
    Task<IList<string>> GetDistricts();
}