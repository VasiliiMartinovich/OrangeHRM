using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class ShortlistCandidatePage : LoadablePage
{
    private static readonly By ShortlistCandidateHeader = By.XPath("//h6[text()='Shortlist Candidate']");
    
    [FindsBy(How = How.CssSelector, Using = "button[type='submit']")]
    private IWebElement _saveButton = null!;
    
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