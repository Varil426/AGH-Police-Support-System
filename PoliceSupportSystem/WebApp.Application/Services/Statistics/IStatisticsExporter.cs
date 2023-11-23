namespace WebApp.Application.Services.Statistics;

public interface IStatisticsExporter
{
    public void ExportAsZip(Stream stream);
}