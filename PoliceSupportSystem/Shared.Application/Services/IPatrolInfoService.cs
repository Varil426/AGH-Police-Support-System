namespace Shared.Application.Services;

public interface IPatrolInfoService
{
    string PatrolId { get; }
    Guid PatrolAgentId { get; }
    Guid NavAgentId { get; }
    Guid GunAgentId { get; }
}