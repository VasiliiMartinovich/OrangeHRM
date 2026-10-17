using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class RecruitmentPage : LoadablePage
{
    private static readonly By RecruitmentHeader = By.XPath("//h6[text()='Recruitment']");

    [FindsBy(How = How.XPath, Using = "//a[text()='Candidates']")]
    private IWebElement _candidatesTab = null!;

    [FindsBy(How = How.XPath, Using = "//a[text()='Vacancies']")]
    private IWebElement _vacanciesTab = null!;

    public RecruitmentPage() : base(RecruitmentHeader)
    {
        PageFactory.InitElements(Driver, this);
    }

    public bool IsDisplayed()
    {
        return FindVisibleElement(RecruitmentHeader).Displayed;
    }

    public CandidatesPage OpenCandidates()
    {
        Wait.Until(driver =>
            _candidatesTab.Displayed && _candidatesTab.Enabled);

        _candidatesTab.Click();

        return new CandidatesPage();
    }

    public VacanciesPage OpenVacancies()
    {
        Wait.Until(driver =>
            _vacanciesTab.Displayed && _vacanciesTab.Enabled);

        _vacanciesTab.Click();

        return new VacanciesPage();
    }
}