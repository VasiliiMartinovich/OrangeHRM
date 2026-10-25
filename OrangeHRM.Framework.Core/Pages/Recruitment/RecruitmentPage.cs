using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class RecruitmentPage : LoadablePage
{
    private static readonly By RecruitmentHeader = By.XPath("//h6[text()='Recruitment']");
    private readonly Button _candidatesTab = new(By.XPath("//a[text()='Candidates']"));
    private readonly Button _vacanciesTab = new(By.XPath("//a[text()='Vacancies']"));

    public RecruitmentPage() : base(RecruitmentHeader)
    {
    }

    public bool IsDisplayed()
    {
        return FindVisibleElement(RecruitmentHeader).Displayed;
    }

    public CandidatesPage OpenCandidates()
    {
        Wait.Until(driver => _candidatesTab.IsDisplayed() && _candidatesTab.IsEnabled());

        _candidatesTab.Click();

        return new CandidatesPage();
    }

    public VacanciesPage OpenVacancies()
    {
        Wait.Until(driver => _vacanciesTab.IsDisplayed() && _vacanciesTab.IsEnabled());

        _vacanciesTab.Click();

        return new VacanciesPage();
    }
}