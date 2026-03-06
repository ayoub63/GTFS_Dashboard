using System.ComponentModel.DataAnnotations;

namespace GtfsDashboard.Api.Models;

public class Route
{
    [Key]
    [MaxLength(128)]
    public string RouteId { get; set; } = string.Empty;

    [MaxLength(128)]
    public string? AgencyId { get; set; }

    [MaxLength(64)]
    public string? RouteShortName { get; set; }

    [MaxLength(256)]
    public string? RouteLongName { get; set; }

    public int? RouteType { get; set; }
}