using Shared.Application.Integration.DTOs;

namespace HqService.Application.Services.DecisionService;

public record HandleIncidentOrder(Guid Id, string PatrolId, IncidentDto IncidentDto) : PatrolOrder(PatrolId);