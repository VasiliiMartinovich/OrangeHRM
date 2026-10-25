using log4net;
using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Components;

public class SidePanel : BasePage
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(SidePanel));
    
    private readonly Button _pimMenuItem = new(By.XPath("//span[normalize-space()='PIM']"));
    private readonly Button _recruitmentMenuItem = new(By.XPath("//span[normalize-space()='Recruitment']"));
    private readonly Button _adminMenuItem = new(By.XPath("//span[normalize-space()='Admin']"));

    public SidePanel()
    {
    }

    public void OpenPim()
    {
        Wait.Until(driver => _pimMenuItem.IsDisplayed() && _pimMenuItem.IsEnabled());

        _pimMenuItem.Click();
    }

    public void OpenRecruitment()
    {
        Wait.Until(driver =>
            _recruitmentMenuItem.IsDisplayed() &&
            _recruitmentMenuItem.IsEnabled());

        _recruitmentMenuItem.Click();
    }
    
    public void OpenAdmin()
    {
        Wait.Until(driver => _adminMenuItem.IsDisplayed() && _adminMenuItem.IsEnabled());

        _adminMenuItem.Click();
    }
}