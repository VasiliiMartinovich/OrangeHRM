using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OrangeHRM.Framework.Core.Drivers;

namespace OrangeHRM.Framework.Core.Pages;

public class BasePage
{
    protected IWebDriver Driver => DriverManager.Driver;
    protected WebDriverWait Wait { get; }
    
    protected BasePage()
    {
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(DriverManager.TimeoutSeconds));
    }

    protected IWebElement FindVisibleElement(By locator)
    {
        return Wait.Until(driver =>
        {
            var elements = driver.FindElements(locator);

            foreach (var element in elements)
            {
                if (element.Displayed)
                    return element;
            }

            return null;
        })!;
    }

    protected void Click(By locator)
    {
        Wait.Until(driver =>
        {
            var elements = driver.FindElements(locator);

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

    protected void TypeText(By locator, string text)
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
                    return true;
                }
            }

            return false;
        });
    }
}