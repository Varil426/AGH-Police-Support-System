using System.Text;
using Ionic.Zip;

namespace WebApp.Application.Services.Statistics;

internal class StatisticsExporter : IStatisticsExporter
{
    private readonly IStatisticsManager _statisticsManager;

    public StatisticsExporter(IStatisticsManager statisticsManager)
    {
        _statisticsManager = statisticsManager;
    }

    public void ExportAsZip(Stream stream)
    {
        using var zip = new ZipFile();
        zip.AddEntry("GeneralStatistics.csv", ExportGeneralStatistics());
        zip.AddEntry("PatrolPositionHistory.csv", ExportPatrolPositionHistory());
        zip.AddEntry("PatrolStateHistory.csv", ExportPatrolStateHistory());
        zip.AddEntry("IncidentStateHistory.csv", ExportIncidentStateHistory());
        zip.AddEntry("IncidentSummary.csv", ExportIncidentSummary());
        zip.AddEntry("NumberOfIncidentsInTime.csv", ExportNumberOfIncidentsInTime());
        zip.Save(stream);
    }

    private string ExportGeneralStatistics()
    {
        var numberOfPatrols = _statisticsManager.PatrolData.Count;
        var numberOfIncidents = _statisticsManager.IncidentData.Count;
        var numberOfShootings = _statisticsManager.IncidentData.Count(x => x.ChangedIntoFiring);
        var averageDistanceOfConsideredPatrolFromIncident = _statisticsManager.DistancesOfConsideredPatrolsFromIncident.Any() ? _statisticsManager.DistancesOfConsideredPatrolsFromIncident.Average() : 0;
        var averageDistanceOfChosenPatrolFromIncident = _statisticsManager.DistancesOfChosenPatrolsFromIncident.Any() ? _statisticsManager.DistancesOfChosenPatrolsFromIncident.Average() : 0;

        var oldestIncidentData = _statisticsManager.IncidentData.Any() ? _statisticsManager.IncidentData.MinBy(x => x.CreatedAt)?.CreatedAt : null;
        var oldestPatrolData = _statisticsManager.PatrolData.Any() ? _statisticsManager.PatrolData.MinBy(x => x.History.States.First().since)?.History.States.First().since : null;
        DateTimeOffset? dataSince;

        if (oldestIncidentData is not null && oldestPatrolData is not null)
            dataSince = oldestIncidentData < oldestPatrolData ? oldestIncidentData : oldestPatrolData;
        else if (oldestIncidentData is null)
            dataSince = oldestPatrolData;
        else
            dataSince = oldestIncidentData;
        
        
        var newestIncidentData = _statisticsManager.IncidentData.Any() ? _statisticsManager.IncidentData.MaxBy(x => x.History.States.Last().since)?.History.States.Last().since : null;
        var newestPatrolData =  _statisticsManager.PatrolData.Any() ? _statisticsManager.PatrolData.MaxBy(x => x.History.States.Last().since)?.History.States.Last().since : null;
        DateTimeOffset? dataTo;

        if (newestIncidentData is not null && newestPatrolData is not null)
            dataTo = newestIncidentData > newestPatrolData ? newestIncidentData : newestPatrolData;
        else if (newestIncidentData is null)
            dataTo = newestPatrolData;
        else
            dataTo = newestIncidentData;

//         var results = $"""
//                        NumberOfPatrols: {},
//                        NumberOfIncidents: {},
//                        NumberOfShootings:
//                        """;
//
//         return new MemoryStream(Encoding.UTF8.GetBytes(results));

        // using var memoryStream = new MemoryStream();
        // using var streamWriter = new StreamWriter(memoryStream);
        // using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
        //
        // csvWriter.WriteRecord();
        //
        // return memoryStream.ToString() ?? string.Empty;
        
        return $"""
                NumberOfPatrols, NumberOfIncidents, NumberOfShootings, AverageDistanceOfConsideredPatrolFromIncident, AverageDistanceOfChosenPatrolFromIncident, DataSince, DataTo
                {numberOfPatrols},{numberOfIncidents},{numberOfShootings},{averageDistanceOfConsideredPatrolFromIncident},{averageDistanceOfChosenPatrolFromIncident},{dataSince},{dataTo}
                """;
    }

    private string ExportPatrolPositionHistory()
    {
        var header = "Id, Latitude, Longitude, ChangedAt";
        var results = new StringBuilder(header);
        foreach (var patrolData in _statisticsManager.PatrolData)
        {
            foreach (var (position, date) in patrolData.PositionHistory)
            {
                results.AppendLine($"{patrolData.PatrolId},{position.Latitude},{position.Longitude},{date}");
            }
        }

        return results.ToString();
    }

    private string ExportPatrolStateHistory()
    {
        var header = "Id, State, ChangedAt";
        var results = new StringBuilder(header);
        foreach (var patrolData in _statisticsManager.PatrolData)
        {
            foreach (var (state, date) in patrolData.History.States)
            {
                results.AppendLine($"{patrolData.PatrolId},{state},{date}");
            }
        }

        return results.ToString();
    }
    
    private string ExportIncidentStateHistory()
    {
        var header = "Id, State, ChangedAt";
        var results = new StringBuilder(header);
        foreach (var incidentData in _statisticsManager.IncidentData)
        {
            foreach (var (status, date) in incidentData.History.States)
            {
                results.AppendLine($"{incidentData.IncidentId},{status},{date}");
            }
        }

        return results.ToString();
    }

    private string ExportIncidentSummary()
    {
        var header = "Id, Latitude, Longitude, CreatedAt, ResponseAt, ResolvedAt, ChangedIntoFiring";
        var results = new StringBuilder(header);
        foreach (var incidentData in _statisticsManager.IncidentData)
        {
            results.AppendLine($"{incidentData.IncidentId},{incidentData.Position.Latitude},{incidentData.Position.Longitude},{incidentData.CreatedAt},{incidentData.ResponseAt}, {incidentData.ResolvedAt}, {incidentData.ChangedIntoFiring}");
        }

        return results.ToString();
    }

    private string ExportNumberOfIncidentsInTime()
    {
        var header = "NumberOfActiveIncidents, NumberOfActiveShootings, Time";
        var result = new StringBuilder(header);
        foreach (var kv in _statisticsManager.IncidentsInTime)
            result.AppendLine($"{kv.Value.numberOfIncidents}, {kv.Value.numberOfShootings}, {kv.Key}");
        return result.ToString();
    }
    
    private Stream GenerateStreamFromString(string s)
    {
        MemoryStream stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}