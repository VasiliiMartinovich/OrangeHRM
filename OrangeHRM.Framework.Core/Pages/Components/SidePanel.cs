using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace OrangeHRM.Framework.Core.Pages.Components;

public class SidePanel : BasePage
{
    [FindsBy(How = How.XPath, Using = "//span[normalize-space()='PIM']")]
    private IWebElement _pimMenuItem = null!;

    [FindsBy(How = How.XPath, Using = "//span[normalize-space()='Recruitment']")]
    private IWebElement _recruitmentMenuItem = null!;

    [FindsBy(How = How.XPath, Using = "//span[normalize-space()='Admin']")]
    private IWebElement _adminMenuItem = null!;

    public SidePanel()
    {
        PageFactory.InitElements(Driver, this);
    }

    public void OpenPim()
    {
        Wait.Until(driver => _pimMenuItem.Displayed && _pimMenuItem.Enabled);
        _pimMenuItem.Click();
    }

    public void OpenRecruitment()
    {
        Wait.Until(driver => _recruitmentMenuItem.Displayed && _recruitmentMenuItem.Enabled);
        _recruitmentMenuItem.Click();
    }

    public void OpenAdmin()
    {
        Wait.Until(driver => _adminMenuItem.Displayed && _adminMenuItem.Enabled);
        _adminMenuItem.Click();
    }
}