using OpenQA.Selenium;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class RecruitmentPage : LoadablePage
{
    private static readonly By RecruitmentHeader = By.XPath("//h6[text()='Recruitment']");
    private static readonly By CandidatesTab = By.XPath("//a[text()='Candidates']");
    private static readonly By VacanciesTab = By.XPath("//a[text()='Vacancies']");
    
    public RecruitmentPage() : base(RecruitmentHeader)
    {
    }
    
    public bool IsDisplayed()
    {
        return FindVisibleElement(RecruitmentHeader).Displayed;
    }

    public CandidatesPage OpenCandidates()
    {
        Click(CandidatesTab);
        return new CandidatesPage();
    }
    
    public VacanciesPage OpenVacancies()
    {
        Click(VacanciesTab);
        return new VacanciesPage();
    }
}