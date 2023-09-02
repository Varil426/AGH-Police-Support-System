using Shared.Application.DTOs;

namespace HqService.Application.Services;

public interface IReportingService
{
    Task ReportNewIncident(NewIncidentDto newIncidentDto);

    Task UpdateStatus(UpdateIncidentDto updateIncidentDto);
}