using Microsoft.Extensions.Options;
using MyAutoBadge.Helpers;
using MyAutoBadge.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyAutoBadge.Services;

public class HolidaysService
{
    private readonly HolidayOptions _options;

    public HolidaysService(IOptions<HolidayOptions> options)
    {
        _options = options.Value;
    }

    public bool IsTodayHoliday(DateTime date)
    {
        var holidays = new List<DateTime>(_options.StaticDates);

        if (_options.EnableEasterBasedHolidays)
            holidays.AddRange(EasterCalculator.GetVariableHolidays(date.Year));

        holidays = holidays.Select(d => d.Date).ToList();

        return holidays.Any(h => h.Date == date.Date)
            || date.DayOfWeek == DayOfWeek.Saturday
            || date.DayOfWeek == DayOfWeek.Sunday;
    }
}