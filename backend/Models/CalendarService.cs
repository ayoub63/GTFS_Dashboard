using System.ComponentModel.DataAnnotations;

namespace GtfsDashboard.Api.Models;

public class CalendarService
{
    [Key]
    [MaxLength(128)]
    public string ServiceId { get; set; } = string.Empty;

    public int Monday { get; set; }
    public int Tuesday { get; set; }
    public int Wednesday { get; set; }
    public int Thursday { get; set; }
    public int Friday { get; set; }
    public int Saturday { get; set; }
    public int Sunday { get; set; }

    [MaxLength(8)]
    public string StartDate { get; set; } = string.Empty;

    [MaxLength(8)]
    public string EndDate { get; set; } = string.Empty;
}