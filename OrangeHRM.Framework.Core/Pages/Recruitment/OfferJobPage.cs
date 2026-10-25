using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class OfferJobPage : LoadablePage
{
    private static readonly By OfferJobHeader = By.XPath("//h6[text()='Offer Job']");
    private readonly Button _saveButton = new(By.CssSelector("button[type='submit']"));
    
    public OfferJobPage() : base(OfferJobHeader)
    {
    }

    public CandidateDetailsPage SaveButtonClick()
    {
        _saveButton.Click();

        return new CandidateDetailsPage();
    }
}