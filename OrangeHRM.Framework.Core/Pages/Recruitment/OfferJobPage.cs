using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class OfferJobPage : LoadablePage
{
    private static readonly By OfferJobHeader = By.XPath("//h6[text()='Offer Job']");
    private readonly Button _saveButton = new(By.CssSelector("button[type='submit']"));
    // private readonly TextBox _candidate = new(
    //     By.XPath("//label[normalize-space()='Candidate']" +
    //              "/ancestor::div[contains(@class,'oxd-input-group')]//input"));
    //
    // private readonly TextBox _vacancy = new(
    //     By.XPath("//label[normalize-space()='Vacancy']" +
    //              "/ancestor::div[contains(@class,'oxd-input-group')]//input"));
    //
    // private readonly TextBox _hiringManager = new(
    //     By.XPath("//label[normalize-space()='Hiring Manager']" +
    //              "/ancestor::div[contains(@class,'oxd-input-group')]//input"));

  
    public OfferJobPage() : base(OfferJobHeader)
    {
    }

    public CandidateDetailsPage SaveButtonClick()
    {
        _saveButton.Click();

        return new CandidateDetailsPage();
    }
}