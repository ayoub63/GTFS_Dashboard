using System.ComponentModel.DataAnnotations;

namespace GtfsDashboard.Api.Models;

public class StopTime
{
    [Key]
    public long Id { get; set; }

    [MaxLength(128)]
    public string TripId { get; set; } = string.Empty;

    [MaxLength(16)]
    public string? ArrivalTime { get; set; }

    [MaxLength(16)]
    public string? DepartureTime { get; set; }

    [MaxLength(128)]
    public string StopId { get; set; } = string.Empty;

    public int StopSequence { get; set; }
}