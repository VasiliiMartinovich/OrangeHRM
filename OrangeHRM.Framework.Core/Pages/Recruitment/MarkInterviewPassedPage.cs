using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class MarkInterviewPassedPage : LoadablePage
{
    private static readonly By MarkInterviewPassedHeader = By.XPath("//h6[text()='Mark Interview Passed']");
    private readonly Button _saveButton = new(By.CssSelector("button[type='submit']"));

    public MarkInterviewPassedPage() : base(MarkInterviewPassedHeader)
    {
    }

    public CandidateDetailsPage SaveButtonClick()
    {
        _saveButton.Click();

        return new CandidateDetailsPage();
    }
}