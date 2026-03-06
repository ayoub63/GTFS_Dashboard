using System.ComponentModel.DataAnnotations;

namespace GtfsDashboard.Api.Models;

public class CalendarDate
{
    [Key]
    public long Id { get; set; }

    [MaxLength(128)]
    public string ServiceId { get; set; } = string.Empty;

    [MaxLength(8)]
    public string Date { get; set; } = string.Empty;

    public int ExceptionType { get; set; }
}