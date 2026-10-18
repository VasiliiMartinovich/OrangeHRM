using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Drivers;
using SeleniumExtras.PageObjects;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class CandidateDetailsPage : LoadablePage
{
    private static readonly By CandidateDetailsHeader = By.XPath("//h6[text()='Candidate Profile']");
    private static readonly By ResumeFile = By.CssSelector("p.orangehrm-file-name[title='candidate-cv.pdf']");
    private static readonly By CandidateStatus = By.CssSelector(".orangehrm-recruitment-status p");

    [FindsBy(How = How.CssSelector, Using = "input[name='firstName']")]
    private IWebElement _firstName = null!;

    [FindsBy(How = How.CssSelector, Using = "input[name='middleName']")]
    private IWebElement _middleName = null!;

    [FindsBy(How = How.CssSelector, Using = "input[name='lastName']")]
    private IWebElement _lastName = null!;

    [FindsBy(
        How = How.XPath,
        Using = "//label[normalize-space()='Email']/ancestor::div[contains(@class,'oxd-input-group')]//input")]
    private IWebElement _email = null!;

    [FindsBy(
        How = How.XPath,
        Using = "//label[normalize-space()='Job Vacancy']/ancestor::div[contains(@class,'oxd-input-group')]//div[contains(@class,'oxd-select-text-input')]")]
    private IWebElement _jobVacancy = null!;
    
    [FindsBy(How = How.XPath, Using = "//button[normalize-space()='Shortlist']")]
    private IWebElement _shortlistButton = null!;
    
    [FindsBy(How = How.XPath, Using = "//button[normalize-space()='Schedule Interview']")]
    private IWebElement _scheduleInterviewButton = null!;
    
    [FindsBy(How = How.XPath, Using = "//button[normalize-space()='Mark Interview Passed']")]
    private IWebElement _markInterviewPassedButton = null!;
    
    [FindsBy(How = How.XPath, Using = "//button[normalize-space()='Offer Job']")]
    private IWebElement _offerJobButton = null!;
    
    [FindsBy(How = How.XPath, Using = "//button[normalize-space()='Hire']")]
    private IWebElement _hireButton = null!;

    public CandidateDetailsPage() : base(CandidateDetailsHeader)
    {
        PageFactory.InitElements(Driver, this);
    }

    public bool IsDisplayed()
    {
        return FindVisibleElement(CandidateDetailsHeader).Displayed;
    }

    public string GetFirstName()
    {
        return _firstName.GetAttribute("value") ?? string.Empty;
    }

    public string GetMiddleName()
    {
        return _middleName.GetAttribute("value") ?? string.Empty;
    }

    public string GetLastName()
    {
        return _lastName.GetAttribute("value") ?? string.Empty;
    }

    public string GetEmail()
    {
        return _email.GetAttribute("value") ?? string.Empty;
    }

    public string GetJobVacancy()
    {
        return _jobVacancy.Text.Trim();
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
        return FindVisibleElement(CandidateStatus)
            .Text
            .Replace("Status:", "")
            .Trim();
    }

    public ScheduleInterviewPage ClickScheduleInterviewButton()
    {
        _scheduleInterviewButton.Click();
        return new ScheduleInterviewPage();
    }
    
    public MarkInterviewPassedPage CLickMarkInterviewPassedButton()
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
