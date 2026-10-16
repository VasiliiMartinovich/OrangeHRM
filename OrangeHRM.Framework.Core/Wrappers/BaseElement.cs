using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OrangeHRM.Framework.Core.Drivers;

namespace OrangeHRM.Framework.Core.Wrappers;

public class BaseElement
{
    protected IWebDriver Driver => DriverManager.Driver;

    protected WebDriverWait Wait => new WebDriverWait(Driver,
            TimeSpan.FromSeconds(DriverManager.TimeoutSeconds));

    protected By Locator { get; }

    public BaseElement(By locator)
    {
        Locator = locator;
    }

    public void Click()
    {
        Wait.Until(driver =>
        {
            var elements = driver.FindElements(Locator);

            foreach (var element in elements)
            {
                if (element.Displayed && element.Enabled)
                {
                    element.Click();
                    return true;
                }
            }

            return false;
        });
    }

    public void SetValue(string value)
    {
        Wait.Until(driver =>
        {
            var elements = driver.FindElements(Locator);

            foreach (var element in elements)
            {
                if (element.Displayed && element.Enabled)
                {
                    element.Clear();
                    element.SendKeys(value);
                    return true;
                }
            }

            return false;
        });
    }

    public string GetText()
    {
        return Wait.Until(driver =>
        {
            var elements = driver.FindElements(Locator);

            foreach (var element in elements)
            {
                if (element.Displayed)
                {
                    return element.Text;
                }
            }

            return null;
        })!;
    }

    public bool IsDisplayed()
    {
        return Driver.FindElements(Locator)
            .Any(element => element.Displayed);
    }

    public string? GetAttribute(string attributeName)
    {
        return FindVisibleElement().GetAttribute(attributeName);
    }

    protected IWebElement FindVisibleElement()
    {
        return Wait.Until(driver =>
        {
            var elements = driver.FindElements(Locator);

            foreach (var element in elements)
            {
                if (element.Displayed)
                {
                    return element;
                }
            }
            return null;
        })!;
    }
    
    public bool IsEnabled()
    {
        return FindVisibleElement().Enabled;
    }

    public void Clear()
    {
        Wait.Until(driver =>
        {
            var elements = driver.FindElements(Locator);

            foreach (var element in elements)
            {
                if (element.Displayed && element.Enabled)
                {
                    element.Clear();
                    return true;
                }
            }

            return false;
        });
    }
}
