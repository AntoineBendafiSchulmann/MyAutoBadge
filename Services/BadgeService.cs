using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAutoBadge.Helpers;
using MyAutoBadge.Models;
using System;
using System.Threading.Tasks;

namespace MyAutoBadge.Services;

public class BadgeService
{
    private readonly ILogger<BadgeService> _logger;
    private readonly AutomationOptions _automationOptions;
    private readonly HolidayOptions _holidayOptions;
    private readonly SessionLockService _sessionLockService;
    private readonly HolidaysService _holidaysService;
    private readonly WebAutomationService _webAutomationService;

    public BadgeService(
        ILogger<BadgeService> logger,
        IOptions<AutomationOptions> automationOptions,
        IOptions<HolidayOptions> holidayOptions,
        SessionLockService sessionLockService,
        HolidaysService holidaysService,
        WebAutomationService webAutomationService)
    {
        _logger = logger;
        _automationOptions = automationOptions.Value;
        _holidayOptions = holidayOptions.Value;
        _sessionLockService = sessionLockService;
        _holidaysService = holidaysService;
        _webAutomationService = webAutomationService;
    }

    public async Task TryBadgeAsync()
    {
        _logger.LogInformation("BadgeService.TryBadgeAsync() déclenché à {Now}", DateTime.Now);

        if (_sessionLockService.IsLocked())
        {
            _logger.LogInformation("session verrouillée détectée. badgeage ignoré");
            return;
        }

        var now = DateTime.Now;

        if (now.Hour < _automationOptions.StartHour || now.Hour > _automationOptions.EndHour)
        {
            _logger.LogInformation("heure actuelle hors plage autorisée ({Start}h–{End}h).", _automationOptions.StartHour, _automationOptions.EndHour);
            return;
        }

        bool isHolidayOrWeekend = _holidaysService.IsTodayHoliday(now);
        bool isWeekend = now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday;

        if (!_automationOptions.AllowWeekends && isHolidayOrWeekend)
        {
            if (isWeekend)
                _logger.LogInformation("Week-end détecté. Badge refusé car AllowWeekends = false");
            else
                _logger.LogInformation("jour férié détecté, badgeage refusé");

            return;
        }

        _logger.LogInformation("conditions remplies, lancement du badgeage");
        await _webAutomationService.BadgeAsync();
        _logger.LogInformation("badgeage effectué avec succès");
    }
}