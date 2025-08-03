using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAutoBadge.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyAutoBadge.Services;

public class DailyBadgeWorker : BackgroundService
{
    private readonly ILogger<DailyBadgeWorker> _logger;
    private readonly BadgeService _badgeService;
    private readonly AutomationOptions _options;

    public DailyBadgeWorker(
        ILogger<DailyBadgeWorker> logger,
        BadgeService badgeService,
        IOptions<AutomationOptions> options)
    {
        _logger = logger;
        _badgeService = badgeService;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("badgeage automatique configuré à {Time}.", _options.DailyBadgeTime);

        while (!stoppingToken.IsCancellationRequested)
        {
            DateTime now = DateTime.Now;
            DateTime targetTime = now.Date + _options.DailyBadgeTime;

            if (now >= targetTime)
                targetTime = targetTime.AddDays(1);

            TimeSpan delay = targetTime - now;

            _logger.LogInformation("prochain badgeage prévu à : {Target}", targetTime);

            try
            {
                await Task.Delay(delay, stoppingToken);
                if (stoppingToken.IsCancellationRequested) break;

                _logger.LogInformation("heure atteinte, tentative de badge");
                await _badgeService.TryBadgeAsync();
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("arrêt du badge automatique en cours");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "erreur lors du badgeage automatique");
            }
        }
    }
}