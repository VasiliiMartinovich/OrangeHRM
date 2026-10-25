using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class HireCandidatePage : LoadablePage
{
    private static readonly By HireCandidateHeader =
        By.XPath("//h6[text()='Hire Candidate']");

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

    private readonly Button _saveButton =
        new(By.CssSelector("button[type='submit']"));

    public HireCandidatePage() : base(HireCandidateHeader)
    {
    }

    public CandidateDetailsPage SaveButtonClick()
    {
        _saveButton.Click();

        return new CandidateDetailsPage();
    }
}