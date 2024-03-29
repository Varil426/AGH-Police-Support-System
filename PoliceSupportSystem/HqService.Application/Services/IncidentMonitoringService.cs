﻿using Shared.Application.Helpers;
using Shared.Application.Integration.DTOs;
using Shared.CommonTypes.Incident;
using Shared.Domain.Incident;

namespace HqService.Application.Services;

public class IncidentMonitoringService : IIncidentMonitoringService
{
    private Dictionary<Guid, Incident> _incidents = new();

    private SemaphoreSlim _semaphore = new(1, 1);

    public IncidentMonitoringService()
    {
    }

    public async Task<IReadOnlySet<Incident>> GetOnGoingIncidents()
    {
        await _semaphore.WaitAsync();
        try
        {
            return _incidents.Values.Where(x => x.Status != IncidentStatusEnum.Resolved).ToHashSet();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<IReadOnlySet<Incident>> GetIncidents()
    {
        await _semaphore.WaitAsync();
        try
        {
            return _incidents.Values.ToHashSet();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task AddIncident(Incident incident)
    {
        await _semaphore.WaitAsync();
        try
        {
            _incidents.Add(incident.Id, incident);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<Incident?> GetIncidentById(Guid id)
    {
        await _semaphore.WaitAsync();
        try
        {
            return _incidents.TryGet(id);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task UpdatedIncident(UpdateIncidentDto updateIncidentDto)
    {
        var incident = await GetIncidentById(updateIncidentDto.Id) ?? throw new Exception($"Incident with id: {updateIncidentDto.Id} not found");
        incident.UpdateStatus(updateIncidentDto.NewIncidentStatus);
        incident.UpdateLocation(updateIncidentDto.NewLocation ?? incident.Location);
        incident.UpdateType(updateIncidentDto.NewIncidentType);
    }
}