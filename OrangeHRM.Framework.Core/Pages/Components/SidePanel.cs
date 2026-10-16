using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Components;

public class SidePanel : BasePage
{
    private readonly Button _pimMenuItem = new(By.XPath("//span[normalize-space()='PIM']"));
    private readonly Button _recruitmentMenuItem = new(By.XPath("//span[normalize-space()='Recruitment']"));
    private readonly Button _adminMenuItem = new(By.XPath("//span[normalize-space()='Admin']"));

    public void OpenPim()
    {
        _pimMenuItem.Click();
    }

    public void OpenRecruitment()
    {
        _recruitmentMenuItem.Click();
    }

    public void OpenAdmin()
    {
        _adminMenuItem.Click();
    }
}