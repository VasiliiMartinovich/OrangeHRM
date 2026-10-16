using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Safari;

namespace OrangeHRM.Framework.Core.Drivers;

public class DriverFactory
{
    public static IWebDriver CreateDriver(
        BrowserType browserType,
        bool headless = false)
    {
        return browserType switch
        {
            BrowserType.Chrome => CreateChrome(headless),
            BrowserType.Firefox => CreateFirefox(headless),
            BrowserType.Safari => CreateSafari(),
            _ => throw new ArgumentOutOfRangeException(
                nameof(browserType),
                browserType,
                "Unsupported browser type")
        };
    }

    private static IWebDriver CreateChrome(bool headless)
    {
        var options = new ChromeOptions();

        if (headless)
        {
            options.AddArgument("--headless=new");
        }

        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--guest");

        return new ChromeDriver(options);
    }

    private static IWebDriver CreateFirefox(bool headless)
    {
        var options = new FirefoxOptions();

        if (headless)
        {
            options.AddArgument("-headless");
        }

        return new FirefoxDriver(options);
    }

    private static IWebDriver CreateSafari()
    {
        var options = new SafariOptions();

        return new SafariDriver(options);
    }
}