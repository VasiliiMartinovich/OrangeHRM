using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class ShortlistCandidatePage : LoadablePage
{
    private static readonly By ShortlistCandidateHeader = By.XPath("//h6[text()='Shortlist Candidate']");
    private readonly Button _saveButton = new(By.CssSelector("button[type='submit']"));

    public ShortlistCandidatePage() : base(ShortlistCandidateHeader)
    {
    }

    public bool IsDisplayed()
    {
        return FindVisibleElement(ShortlistCandidateHeader).Displayed;
    }

    public CandidateDetailsPage SaveButtonClick()
    {
        _saveButton.Click();

        return new CandidateDetailsPage();
    }
}