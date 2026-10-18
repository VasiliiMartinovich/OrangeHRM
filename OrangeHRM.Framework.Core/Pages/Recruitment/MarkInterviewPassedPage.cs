using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class MarkInterviewPassedPage : LoadablePage
{
    private static readonly By MarkInterviewPassedHeader = By.XPath("//h6[text()='Mark Interview Passed']");

    [FindsBy(How = How.XPath,
        Using = "//label[normalize-space()='Candidate']/ancestor::div[contains(@class,'oxd-input-group')]//input")]
    private IWebElement _candidate = null!;

    [FindsBy(How = How.XPath,
        Using = "//label[normalize-space()='Vacancy']/ancestor::div[contains(@class,'oxd-input-group')]//input")]
    private IWebElement _vacancy = null!;

    [FindsBy(How = How.XPath,
        Using = "//label[normalize-space()='Hiring Manager']/ancestor::div[contains(@class,'oxd-input-group')]//input")]
    private IWebElement _hiringManager = null!;

    [FindsBy(How = How.CssSelector, Using = "button[type='submit']")]
    private IWebElement _saveButton = null!;

    public MarkInterviewPassedPage() : base(MarkInterviewPassedHeader)
    {
    }

    public CandidateDetailsPage SaveButtonClick()
    {
        _saveButton.Click();
        return new CandidateDetailsPage();
    }
}