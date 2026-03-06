using GtfsDashboard.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GtfsDashboard.Api.Controllers;

[ApiController]
[Route("api/stops")]
public class StopsController(IStopService stopService) : ControllerBase
{
    // GET: api/stops
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] int limit = 50, [FromQuery] int offset = 0, CancellationToken ct = default)
    {
        var result = await stopService.SearchAsync(search, limit, offset, ct);
        return Ok(result);
    }

    // GET: api/stops/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct = default)
    {
        var stop = await stopService.GetByIdAsync(id, ct);
        if (stop == null) return NotFound();
        
        return Ok(stop);
    }

    // GET: api/stops/nearby
    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearby([FromQuery] double lat, [FromQuery] double lon, [FromQuery] int radiusM = 1000, [FromQuery] int limit = 50, CancellationToken ct = default)
    {
        var result = await stopService.GetNearbyAsync(lat, lon, radiusM, limit, ct);
        return Ok(result);
    }

    // GET: api/stops/in-bbox
    [HttpGet("in-bbox")]
    public async Task<IActionResult> GetInBbox([FromQuery] double minLat, [FromQuery] double minLon, [FromQuery] double maxLat, [FromQuery] double maxLon, [FromQuery] int limit = 1000, CancellationToken ct = default)
    {
        var result = await stopService.GetInBboxAsync(minLat, minLon, maxLat, maxLon, limit, ct);
        return Ok(result);
    }
}