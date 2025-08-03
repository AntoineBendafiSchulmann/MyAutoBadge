using Microsoft.Playwright;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MyAutoBadge.Services;

public class WebAutomationService
{
    private readonly ILogger<WebAutomationService> _logger;

    public WebAutomationService(ILogger<WebAutomationService> logger)
    {
        _logger = logger;
    }

    public async Task BadgeAsync()
    {
        var url = Environment.GetEnvironmentVariable("BADGE_URL");
        var badgeButton = Environment.GetEnvironmentVariable("BADGE_BUTTON_TITLE");
        var confirmButton = Environment.GetEnvironmentVariable("CONFIRM_BUTTON_CLASS");
        var validateButton = Environment.GetEnvironmentVariable("VALIDATE_BUTTON_SELECTOR");

        if (string.IsNullOrWhiteSpace(url) ||
            string.IsNullOrWhiteSpace(badgeButton) ||
            string.IsNullOrWhiteSpace(confirmButton) ||
            string.IsNullOrWhiteSpace(validateButton))
        {
            _logger.LogError("une ou plusieurs variables d'environnement requises sont manquantes ou vides");
            return;
        }

        _logger.LogInformation("initialisation de Playwright...");

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        _logger.LogInformation("navigation vers {Url}", url);
        await page.GotoAsync(url);

        await page.WaitForSelectorAsync($"button[title='{badgeButton}']");
        await page.ClickAsync($"button[title='{badgeButton}']");
        await page.WaitForTimeoutAsync(3000); // délai

        await page.WaitForSelectorAsync(confirmButton);
        await page.ClickAsync(confirmButton);
        await page.WaitForTimeoutAsync(3000);

        await page.WaitForSelectorAsync(validateButton);
        await page.ClickAsync(validateButton);
        await page.WaitForTimeoutAsync(3000);

        _logger.LogInformation("badgeage effectué avec succès");
    }
}