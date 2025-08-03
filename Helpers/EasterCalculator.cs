using System;
using System.Collections.Generic;

namespace MyAutoBadge.Helpers;

public static class EasterCalculator
{
    //calcule la date exacte de paques pour une année donné
    public static DateTime GetEasterDate(int year)
    {
        int g = year % 19;
        int c = year / 100;
        int h = (c - c / 4 - (8 * c + 13) / 25 + 19 * g + 15) % 30;
        int i = h - h / 28 * (1 - h / 28 * (29 / (h + 1)) * ((21 - g) / 11));
        int j = (year + year / 4 + i + 2 - c + c / 4) % 7;
        int l = i - j;
        int month = 3 + (l + 40) / 44;
        int day = l + 28 - 31 * (month / 4);
        return new DateTime(year, month, day);
    }

    public static List<DateTime> GetVariableHolidays(int year)
    {
        var easter = GetEasterDate(year);
        return new List<DateTime>
        {
            easter.AddDays(1),
            easter.AddDays(39),
            easter.AddDays(50),
        };
    }
}