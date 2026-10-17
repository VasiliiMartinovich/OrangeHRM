using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class CandidateDetailsPage : LoadablePage
{
    private static readonly By CandidateDetailsHeader = By.XPath("//h6[text()='Application Stage']");
    private static readonly By ResumeFile = By.CssSelector("p.orangehrm-file-name[title='candidate-cv.pdf']");

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
}
