using WebApp.Application.Services.Statistics;

namespace WebApp.API.Handlers;

public class GetStatisticsHandler //: ControllerBase
{
    private readonly IStatisticsExporter _statisticsExporter;

    public GetStatisticsHandler(IStatisticsExporter statisticsExporter)
    {
        _statisticsExporter = statisticsExporter;
    }

    public IResult Handle()
    {
        using var memoryStream = new MemoryStream();
        _statisticsExporter.ExportAsZip(memoryStream);

        memoryStream.Seek(0, SeekOrigin.Begin);
        var downloadName = $"{DateTimeOffset.UtcNow.ToString("g")}.zip";
        var contentType = "application/octet-stream";

        return Results.File(memoryStream.ToArray(), contentType, downloadName);
    }
}