namespace GtfsDashboard.Api.Services;

public interface IGtfsImportService
{
    Task ImportFromFolderAsync(string dataFolderPath, bool includeAnalyticsFiles, CancellationToken ct);
}