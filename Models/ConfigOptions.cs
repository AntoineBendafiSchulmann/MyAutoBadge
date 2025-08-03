using System;
using System.Collections.Generic;

namespace MyAutoBadge.Models;

public class AutomationOptions
{
    public string BadgeUrl { get; set; } = "";
    public int StartHour { get; set; }
    public int EndHour { get; set; }
    public int UpdateIntervalMinSeconds { get; set; }
    public int UpdateIntervalMaxSeconds { get; set; }
    public TimeSpan DailyBadgeTime { get; set; }
    public bool AllowWeekends { get; set; } = false;
}

public class HolidayOptions
{
    public List<DateTime> StaticDates { get; set; } = new();
    public bool EnableEasterBasedHolidays { get; set; } = true;
}