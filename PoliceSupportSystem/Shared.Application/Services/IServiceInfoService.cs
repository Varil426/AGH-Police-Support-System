using Shared.Application.Common;

namespace Shared.Application.Services;

public interface IServiceInfoService
{
    string Id { get; }
    
    ServiceTypeEnum ServiceType { get; }
}