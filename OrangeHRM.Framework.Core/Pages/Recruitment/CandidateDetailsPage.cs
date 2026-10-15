using OpenQA.Selenium;

namespace OrangeHRM.Framework.UI.Pages.Recruitment;

public class CandidateDetailsPage : LoadablePage
{
    private static readonly By CandidateDetailsHeader = By.XPath("//h6[text()='Application Stage']");
    private static readonly By FirstName = By.CssSelector("input[name='firstName']");
    private static readonly By MiddleName = By.CssSelector("input[name='middleName']");
    private static readonly By LastName = By.CssSelector("input[name='lastName']");
    private static readonly By Email = By.XPath("//label[normalize-space()='Email']/ancestor::div[contains(@class,'oxd-input-group')]//input");
    private static readonly By ResumeFile = By.CssSelector("p.orangehrm-file-name[title='candidate-cv.pdf']");
    private static readonly By JobVacancy =
        By.XPath(
            "//label[normalize-space()='Job Vacancy']" +
            "/ancestor::div[contains(@class,'oxd-input-group')]" +
            "//div[contains(@class,'oxd-select-text-input')]");
    public CandidateDetailsPage() : base(CandidateDetailsHeader)
    {
    }
    public bool IsDisplayed()
    {
        return FindVisibleElement(CandidateDetailsHeader).Displayed;
    }
    public string GetFirstName()
    {
        return Driver.FindElement(FirstName)
            .GetAttribute("value") ?? string.Empty;
    }

    public string GetMiddleName()
    {
        return Driver.FindElement(MiddleName)
            .GetAttribute("value") ?? string.Empty;
    }

    public string GetLastName()
    {
        return Driver.FindElement(LastName)
            .GetAttribute("value") ?? string.Empty;
    }

    public string GetEmail()
    {
        return Driver.FindElement(Email)
            .GetAttribute("value") ?? string.Empty;
    }

    public string GetJobVacancy()
    {
        return FindVisibleElement(JobVacancy).Text.Trim();
    }
    public bool IsResumeDisplayed(string fileName)
    {
        var file = FindVisibleElement(ResumeFile);

        return file.GetAttribute("title") == fileName;
    }
}