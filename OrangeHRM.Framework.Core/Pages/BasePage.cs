using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OrangeHRM.Framework.Core.Drivers;

namespace OrangeHRM.Framework.UI.Pages;

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
            var element = driver.FindElement(locator);
            return element.Displayed ? element : null;
        })!;
    }

    protected void Click(By locator)
    {
        FindVisibleElement(locator).Click();
    }

    protected void TypeText(By locator, string text)
    {
        var element = FindVisibleElement(locator);
        element.Clear();
        element.SendKeys(text);
    }
}