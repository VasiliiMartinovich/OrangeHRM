using OpenQA.Selenium;

namespace OrangeHRM.Framework.UI.Pages.Components;

public class SidePanel : BasePage
{
    private static readonly By PIMMenuItem =
        By.XPath("//span[normalize-space()='PIM']");

    private static readonly By RecruitmentMenuItem =
        By.XPath("//span[normalize-space()='Recruitment']");

    private static readonly By AdminMenuItem =
        By.XPath("//span[normalize-space()='Admin']");

    public void OpenPim()
    {
        Click(PIMMenuItem);
    }

    public void OpenRecruitment()
    {
        Click(RecruitmentMenuItem);
    }

    public void OpenAdmin()
    {
        Click(AdminMenuItem);
    }
}