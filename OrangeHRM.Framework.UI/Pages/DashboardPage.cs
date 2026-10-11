using OpenQA.Selenium;
using OrangeHRM.Framework.UI.Pages.Authentication;

namespace OrangeHRM.Framework.UI.Pages;

public class DashboardPage : LoadablePage
{
    private static readonly By DashboardHeader = By.XPath("//h6[text()='Dashboard']");
    private static readonly By PimMenuItem = By.XPath("//span[text()='PIM']");
    private static readonly By NavigationMenu = By.CssSelector("span.oxd-userdropdown-tab");
    private static readonly By LogoutButton = By.XPath("//a[normalize-space()='Logout']");
    
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

    public DashboardPage  OpenNavigationMenu()
    {
        Click(NavigationMenu);
        return this;
    }
    
    public LoginPage Logout()
    {
        Click(LogoutButton);
        return new LoginPage();
    }
}