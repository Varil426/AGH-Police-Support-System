using HqService.Application.Services;
using Shared.Application.Integration.DTOs;
using Shared.Simulation.Handlers;
using Simulation.Shared.Communication;

namespace HqService.Simulation.Handlers;

public class NewIncidentHandler : ISimulationMessageHandler<NewIncidentMessage>
{
    private readonly IReportingService _reportingService;

    public NewIncidentHandler(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    public Task Handle(NewIncidentMessage simulationMessage) => _reportingService.ReportNewIncident(new NewIncidentDto(simulationMessage.Id, simulationMessage.Location, simulationMessage.Type, simulationMessage.Status));
}