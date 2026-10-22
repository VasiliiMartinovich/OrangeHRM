using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Pages.Authentication;
using OrangeHRM.Framework.Core.Pages.Components;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages;

public class DashboardPage : LoadablePage
{
    private static readonly By DashboardHeader =
        By.XPath("//h6[text()='Dashboard']");

    private readonly Button _navigationMenu =
        new(By.CssSelector("span.oxd-userdropdown-tab"));

    private readonly Button _logoutButton =
        new(By.XPath("//a[normalize-space()='Logout']"));

    public DashboardPage() : base(DashboardHeader)
    {
    }

    public SidePanel SidePanel => new SidePanel();

    public bool IsDisplayed()
    {
        return FindVisibleElement(DashboardHeader).Displayed;
    }

    public DashboardPage OpenNavigationMenu()
    {
        Wait.Until(driver => _navigationMenu.IsDisplayed());
        _navigationMenu.Click();

        return this;
    }

    public LoginPage Logout()
    {
        Wait.Until(driver => _logoutButton.IsDisplayed());
        _logoutButton.Click();

        return new LoginPage();
    }
}