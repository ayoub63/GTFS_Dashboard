using System.ComponentModel.DataAnnotations;

namespace GtfsDashboard.Api.Models;

public class Trip
{
    [Key]
    [MaxLength(128)]
    public string TripId { get; set; } = string.Empty;

    [MaxLength(128)]
    public string RouteId { get; set; } = string.Empty;

    [MaxLength(128)]
    public string ServiceId { get; set; } = string.Empty;

    [MaxLength(256)]
    public string? TripHeadsign { get; set; }

    public int? DirectionId { get; set; }
}