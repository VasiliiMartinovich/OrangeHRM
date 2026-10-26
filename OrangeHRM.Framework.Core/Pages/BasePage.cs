using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OrangeHRM.Framework.Core.Drivers;

namespace OrangeHRM.Framework.Core.Pages;

public class BasePage
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(BasePage));

    protected IWebDriver Driver => DriverManager.Driver;
    protected WebDriverWait Wait { get; }

    protected BasePage()
    {
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(DriverManager.TimeoutSeconds));

        Log.Debug(
            $"Page object initialized: {GetType().Name}. " +
            $"Timeout: {DriverManager.TimeoutSeconds} seconds.");
    }

    protected IWebElement FindVisibleElement(By locator)
    {
        Log.Debug($"{GetType().Name}: waiting for visible element. Locator: {locator}.");

        try
        {
            var element = Wait.Until(driver =>
            {
                var elements = driver.FindElements(locator);

                foreach (var candidate in elements)
                {
                    if (candidate.Displayed)
                        return candidate;
                }

                return null;
            })!;

            Log.Debug($"{GetType().Name}: visible element found. Locator: {locator}.");

            return element;
        }
        catch (WebDriverTimeoutException ex)
        {
            Log.Error(
                $"{GetType().Name}: timeout waiting for visible element. " +
                $"Locator: {locator}.",
                ex);

            throw;
        }
        catch (WebDriverException ex)
        {
            Log.Error(
                $"{GetType().Name}: WebDriver error while finding element. " +
                $"Locator: {locator}.",
                ex);

            throw;
        }
    }

    protected void Click(By locator)
    {
        Log.Debug($"{GetType().Name}: waiting to click element. Locator: {locator}.");

        try
        {
            Wait.Until(driver =>
            {
                var elements = driver.FindElements(locator);

                foreach (var element in elements)
                {
                    if (element.Displayed && element.Enabled)
                    {
                        element.Click();

                        Log.Debug($"{GetType().Name}: click completed. Locator: {locator}.");

                        return true;
                    }
                }

                return false;
            });
        }
        catch (WebDriverTimeoutException ex)
        {
            Log.Error(
                $"{GetType().Name}: timeout waiting to click element. " +
                $"Locator: {locator}.",
                ex);

            throw;
        }
        catch (WebDriverException ex)
        {
            Log.Error(
                $"{GetType().Name}: WebDriver error while clicking element. " +
                $"Locator: {locator}.",
                ex);

            throw;
        }
    }

    protected void TypeText(By locator, string text)
    {
        Log.Debug($"{GetType().Name}: waiting to type into element. Locator: {locator}.");

        try
        {
            Wait.Until(driver =>
            {
                var elements = driver.FindElements(locator);

                foreach (var element in elements)
                {
                    if (element.Displayed && element.Enabled)
                    {
                        element.Clear();
                        element.SendKeys(text);

                        Log.Debug(
                            $"{GetType().Name}: text entered. Locator: {locator}.");

                        return true;
                    }
                }

                return false;
            });
        }
        catch (WebDriverTimeoutException ex)
        {
            Log.Error(
                $"{GetType().Name}: timeout waiting to type into element. " +
                $"Locator: {locator}.",
                ex);

            throw;
        }
        catch (WebDriverException ex)
        {
            Log.Error(
                $"{GetType().Name}: WebDriver error while typing into element. " +
                $"Locator: {locator}.",
                ex);

            throw;
        }
    }
}
