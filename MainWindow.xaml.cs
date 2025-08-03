using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System;
using MyAutoBadge.Services;
using MyAutoBadge.Models;

namespace MyAutoBadge;

public partial class MainWindow : Window
{
    private readonly AutomationOptions _automationOptions;
    private readonly bool _allowWeekends;

    public MainWindow()
    {
        InitializeComponent();

        var options = App.AppHost.Services.GetRequiredService<IOptions<AutomationOptions>>();
        _automationOptions = options.Value;

        var config = App.AppHost.Services.GetRequiredService<IConfiguration>();
        _allowWeekends = bool.TryParse(config["AppSettings:Automation:AllowWeekends"], out var val) && val;

        var now = DateTime.Now;
        var next = now.Date + _automationOptions.DailyBadgeTime;
        if (now > next)
            next = next.AddDays(1);

        AutoBadgeInfo.Text = $"badge automatique actif à {next:HH\\:mm} chaque jour";
        AppendLog($"[info] prochain badgeage prévu à {next:HH\\:mm}");

        UpdateManualBadgeStatus();
    }

    private async void ManualBadgeButton_Click(object sender, RoutedEventArgs e)
    {
        var badgeService = App.AppHost.Services.GetRequiredService<BadgeService>();

        try
        {
            AppendLog("[action] tentative de badge manuel...");

            var now = DateTime.Now;
            var isLocked = App.AppHost.Services.GetRequiredService<SessionLockService>().IsLocked();
            var isHoliday = App.AppHost.Services.GetRequiredService<HolidaysService>().IsTodayHoliday(now);
            var isInTimeRange = now.Hour >= _automationOptions.StartHour && now.Hour <= _automationOptions.EndHour;
            var isWeekend = now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday;

            if (isLocked)
            {
                AppendLog("[info] session verrouillée. badgeage ignoré.");
                MessageBox.Show("badgeage refusé : session verrouillée.");
                return;
            }

            if (!isInTimeRange)
            {
                AppendLog($"[info] hors horaires autorisés ({_automationOptions.StartHour}h–{_automationOptions.EndHour}h).");
                MessageBox.Show("badgeage refusé : hors horaires.");
                return;
            }

            if (!_allowWeekends && (isWeekend || isHoliday))
            {
                AppendLog("[info] Badge bloqué : week-end ou jour férié.");
                MessageBox.Show("Badge refusé : week-end ou jour férié.");
                return;
            }

            await badgeService.TryBadgeAsync();
            AppendLog("[succès] badge manuel effectué.");
            MessageBox.Show("Badge manuel effectué.");
        }
        catch (Exception ex)
        {
            AppendLog($"[erreur] {ex.Message}");
            MessageBox.Show($"erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        UpdateManualBadgeStatus();
    }

    private void UpdateManualBadgeStatus()
    {
        var now = DateTime.Now;
        var isInTimeRange = now.Hour >= _automationOptions.StartHour && now.Hour <= _automationOptions.EndHour;
        var isWeekend = now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday;

        if (!isInTimeRange)
        {
            ManualBadgeStatus.Text = $"badge manuel indisponible : hors horaires ({_automationOptions.StartHour}h–{_automationOptions.EndHour}h)";
            ManualBadgeButton.IsEnabled = false;
            ManualBadgeStatus.Foreground = System.Windows.Media.Brushes.DarkRed;
        }
        else if (!_allowWeekends && isWeekend)
        {
            ManualBadgeStatus.Text = "badge manuel indisponible : week-end";
            ManualBadgeButton.IsEnabled = false;
            ManualBadgeStatus.Foreground = System.Windows.Media.Brushes.DarkRed;
        }
        else
        {
            ManualBadgeStatus.Text = "badge manuel disponible";
            ManualBadgeButton.IsEnabled = true;
            ManualBadgeStatus.Foreground = System.Windows.Media.Brushes.DarkGreen;
        }
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        AppendLog("[info] arrêt");
        MessageBox.Show("application en cours d’arrêt");
        Close();
    }

    private void AppendLog(string text)
    {
        LogBox.AppendText($"{DateTime.Now:HH:mm:ss} - {text}\n");
        LogBox.ScrollToEnd();
    }
}
