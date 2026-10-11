using OpenQA.Selenium;

namespace OrangeHRM.Framework.UI.Pages;

public class DashboardPage : LoadablePage
{
    private static readonly By DashboardHeader = By.XPath("//h6[text()='Dashboard']");
    private static readonly By PimMenuItem = By.XPath("//span[text()='PIM']");
    
    public DashboardPage() : base(DashboardHeader)
    {
    }
    public bool IsDisplayed()
    {
        return FindVisibleElement(DashboardHeader).Displayed;
    }
    
    public void OpenPim()
    {
        var element = Driver.FindElement(PimMenuItem);
        element.Click();
    }
}