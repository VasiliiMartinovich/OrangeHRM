using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Pages.Authentication;
using OrangeHRM.Framework.Core.Pages.Components;

namespace OrangeHRM.Framework.Core.Pages;

public class DashboardPage : LoadablePage
{
    private static readonly By DashboardHeader = By.XPath("//h6[text()='Dashboard']");
    private static readonly By NavigationMenu = By.CssSelector("span.oxd-userdropdown-tab");
    private static readonly By LogoutButton = By.XPath("//a[normalize-space()='Logout']");
    
    
    public DashboardPage() : base(DashboardHeader)
    {
    }
    public SidePanel SidePanel => new SidePanel();
    public bool IsDisplayed()
    {
        return FindVisibleElement(DashboardHeader).Displayed;
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