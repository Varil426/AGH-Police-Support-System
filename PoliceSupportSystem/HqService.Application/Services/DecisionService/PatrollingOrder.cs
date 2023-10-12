namespace HqService.Application.Services.DecisionService;

public record PatrollingOrder(Guid Id, string PatrolId, string DistrictName) : PatrolOrder(PatrolId);