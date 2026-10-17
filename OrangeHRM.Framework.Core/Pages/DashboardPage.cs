using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using OrangeHRM.Framework.Core.Pages.Authentication;
using OrangeHRM.Framework.Core.Pages.Components;

namespace OrangeHRM.Framework.Core.Pages;

public class DashboardPage : LoadablePage
{
    private static readonly By DashboardHeader = By.XPath("//h6[text()='Dashboard']");

    [FindsBy(How = How.CssSelector, Using = "span.oxd-userdropdown-tab")]
    private IWebElement _navigationMenu = null!;

    [FindsBy(How = How.XPath, Using = "//a[normalize-space()='Logout']")]
    private IWebElement _logoutButton = null!;

    public DashboardPage() : base(DashboardHeader)
    {
        PageFactory.InitElements(Driver, this);
    }

    public SidePanel SidePanel => new SidePanel();

    public bool IsDisplayed()
    {
        return FindVisibleElement(DashboardHeader).Displayed;
    }

    public DashboardPage OpenNavigationMenu()
    {
        Wait.Until(driver => _navigationMenu.Displayed);
        _navigationMenu.Click();

        return this;
    }

    public LoginPage Logout()
    {
        Wait.Until(driver => _logoutButton.Displayed);
        _logoutButton.Click();

        return new LoginPage();
    }
}