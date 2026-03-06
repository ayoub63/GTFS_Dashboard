using GtfsDashboard.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GtfsDashboard.Api.Controllers;

[ApiController]
[Route("api/stations")]
public class StationsController(IStopService stopService) : ControllerBase
{
    [HttpGet("{id}/children")]
    public async Task<IActionResult> GetChildren(string id, CancellationToken ct)
    {
        var children = await stopService.GetChildrenAsync(id, ct);
        return Ok(children);
    }
}