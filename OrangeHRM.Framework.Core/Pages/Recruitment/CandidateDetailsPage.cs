using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class CandidateDetailsPage : LoadablePage
{
    private static readonly By CandidateDetailsHeader = By.XPath("//h6[text()='Candidate Profile']");
    private static readonly By ResumeFile = By.CssSelector("p.orangehrm-file-name[title='candidate-cv.pdf']");
    private static readonly By CandidateStatus = By.CssSelector(".orangehrm-recruitment-status p");
    private readonly TextBox _firstName = new(By.CssSelector("input[name='firstName']"));
    private readonly TextBox _middleName = new(By.CssSelector("input[name='middleName']"));
    private readonly TextBox _lastName = new(By.CssSelector("input[name='lastName']"));
    private readonly TextBox _email = new(By.XPath("//label[normalize-space()='Email']/ancestor::div[contains(@class,'oxd-input-group')]//input"));
    private readonly Label _jobVacancy = new(By.XPath("//label[normalize-space()='Job Vacancy']/ancestor::div[contains(@class,'oxd-input-group')]//div[contains(@class,'oxd-select-text-input')]"));
    private readonly Button _shortlistButton = new(By.XPath("//button[normalize-space()='Shortlist']"));
    private readonly Button _scheduleInterviewButton = new(By.XPath("//button[normalize-space()='Schedule Interview']"));
    private readonly Button _markInterviewPassedButton = new(By.XPath("//button[normalize-space()='Mark Interview Passed']"));
    private readonly Button _offerJobButton = new(By.XPath("//button[normalize-space()='Offer Job']"));
    private readonly Button _hireButton = new(By.XPath("//button[normalize-space()='Hire']"));

    public CandidateDetailsPage() : base(CandidateDetailsHeader)
    {
    }

    public bool IsDisplayed()
    {
        return FindVisibleElement(CandidateDetailsHeader).Displayed;
    }

    public string GetFirstName()
    {
        return _firstName.GetValue();
    }

    public string GetMiddleName()
    {
        return _middleName.GetValue();
    }

    public string GetLastName()
    {
        return _lastName.GetValue();
    }

    public string GetEmail()
    {
        return _email.GetValue();
    }

    public string GetJobVacancy()
    {
        return _jobVacancy.GetText().Trim();
    }

    public bool IsResumeDisplayed(string fileName)
    {
        var file = FindVisibleElement(ResumeFile);
        return file.GetAttribute("title") == fileName;
    }

    public ShortlistCandidatePage ShortlistButtonClick()
    {
        _shortlistButton.Click();
        return new ShortlistCandidatePage();
    }
    
    public string GetCandidateStatus()
    {
        return FindVisibleElement(CandidateStatus).Text.Replace("Status:", "").Trim();
    }

    public ScheduleInterviewPage ClickScheduleInterviewButton()
    {
        _scheduleInterviewButton.Click();
        return new ScheduleInterviewPage();
    }
    
    public MarkInterviewPassedPage ClickMarkInterviewPassedButton()
    {
        _markInterviewPassedButton.Click();
        return new MarkInterviewPassedPage();
    }
    
    public OfferJobPage ClickOfferJobButton()
    {
        _offerJobButton.Click();
        return new OfferJobPage();
    }
    
    public HireCandidatePage ClickHireButton()
    {
        _hireButton.Click();
        return new HireCandidatePage();
    }
}
