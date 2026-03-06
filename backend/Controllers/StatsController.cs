using GtfsDashboard.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GtfsDashboard.Api.Controllers;

[ApiController]
[Route("api/stats")]
public class StatsController(IStatsService statsService) : ControllerBase
{
    [HttpGet("top-stops")]
    public async Task<IActionResult> TopStops([FromQuery] DateOnly? date, [FromQuery] DayOfWeek? weekday, [FromQuery] int limit = 10, CancellationToken ct = default)
    {
        if (limit is < 1 or > 100) return BadRequest("limit must be between 1 and 100");
        var data = await statsService.GetTopStopsAsync(date, weekday, limit, ct);
        return Ok(data);
    }

    [HttpGet("peak-hours")]
    public async Task<IActionResult> PeakHours([FromQuery] DateOnly? date, [FromQuery] DayOfWeek? weekday, CancellationToken ct = default)
    {
        var data = await statsService.GetPeakHoursAsync(date, weekday, ct);
        return Ok(data);
    }

    [HttpGet("routes-by-stop/{stopId}")]
    public async Task<IActionResult> RoutesByStop(string stopId, [FromQuery] DateOnly? date, [FromQuery] DayOfWeek? weekday, CancellationToken ct = default)
    {
        var data = await statsService.GetRoutesByStopAsync(stopId, date, weekday, ct);
        return Ok(data);
    }
}