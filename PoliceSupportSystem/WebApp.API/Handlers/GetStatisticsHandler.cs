using WebApp.Application.Services.Statistics;

namespace WebApp.API.Handlers;

public class GetStatisticsHandler //: ControllerBase
{
    private readonly IStatisticsExporter _statisticsExporter;

    public GetStatisticsHandler(IStatisticsExporter statisticsExporter)
    {
        _statisticsExporter = statisticsExporter;
    }

    public /*IHttpActionResult*/ IResult Handle()
    {
        using var memoryStream = new MemoryStream();
        _statisticsExporter.ExportAsZip(memoryStream);

        memoryStream.Seek(0, SeekOrigin.Begin);
        var downloadName = $"{DateTimeOffset.UtcNow.ToString("g")}.zip";
        var contentType = "application/octet-stream";

        return Results.File(memoryStream.ToArray(), contentType, downloadName);

        // var result = new HttpResponseMessage(HttpStatusCode.OK)
        // {
        //     Content = new ByteArrayContent(memoryStream.ToArray())
        // };
        // result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        // {
        //     FileName =  $"{DateTimeOffset.UtcNow.ToString("g")}.zip",
        // };
        // result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //
        // var response = ResponseMessage(result);
        //
        // return response;

        // System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
        // {
        //     FileName = $"{DateTimeOffset.UtcNow.ToString("g")}.zip",
        //     Inline = false  // false = prompt the user for downloading;  true = browser to try to show the file inline
        // };
        // Response.Headers.Add("Content-Disposition", cd.ToString());
        // Response.Headers.Add("X-Content-Type-Options", "nosniff");

        // return File(memoryStream.ToArray(), "application/octet-stream", $"{DateTimeOffset.UtcNow.ToString("g")}.zip");

        // return new FileContentResult(memoryStream.ToArray(), "application/octet-stream")
        // {
        //     FileDownloadName = $"{DateTimeOffset.UtcNow.ToString("g")}.zip",
        //     EnableRangeProcessing = true
        // };
    }
}