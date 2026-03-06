using System.ComponentModel.DataAnnotations;

namespace GtfsDashboard.Api.Models;

public class Stop
{
    [Key]
    [MaxLength(128)]
    public string StopId { get; set; } = string.Empty;

    [MaxLength(256)]
    public string StopName { get; set; } = string.Empty;

    public double StopLat { get; set; }
    public double StopLon { get; set; }
    public int? LocationType { get; set; }

    [MaxLength(128)]
    public string? ParentStation { get; set; }

    [MaxLength(32)]
    public string? PlatformCode { get; set; }
}