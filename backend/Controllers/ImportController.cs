using GtfsDashboard.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GtfsDashboard.Api.Controllers;

[ApiController]
[Route("api/import")]
public class ImportController(IGtfsImportService importer) : ControllerBase
{
    [HttpPost("from-folder")]
    public async Task<IActionResult> Import([FromQuery] string path = "data", [FromQuery] bool includeAnalyticsFiles = true, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(path)) return BadRequest("path is required");

        await importer.ImportFromFolderAsync(path, includeAnalyticsFiles, ct);
        return Ok(new { message = "Import completed", path, includeAnalyticsFiles });
    }
}