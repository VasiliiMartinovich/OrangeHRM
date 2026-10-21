using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class EditVacancyPage : LoadablePage
{
    private static readonly By EditVacancyHeader = By.XPath("//h6[normalize-space()='Edit Vacancy']");

    [FindsBy(
        How = How.XPath,
        Using = "//label[normalize-space()='Vacancy Name']/ancestor::div[contains(@class,'oxd-input-group')]//input")]
    private IWebElement _vacancyNameInput = null!;

    [FindsBy(
        How = How.XPath,
        Using = "//label[normalize-space()='Job Title']/ancestor::div[contains(@class,'oxd-input-group')]//div[contains(@class,'oxd-select-text-input')]")]
    private IWebElement _jobTitleDropdown = null!;

    [FindsBy(
        How = How.XPath,
        Using = "//label[normalize-space()='Hiring Manager']/ancestor::div[contains(@class,'oxd-input-group')]//input")]
    private IWebElement _hiringManagerInput = null!;

    public EditVacancyPage() : base(EditVacancyHeader)
    {
        PageFactory.InitElements(Driver, this);

        Wait.Until(driver =>
            _vacancyNameInput.Displayed
            && !string.IsNullOrWhiteSpace(
                _vacancyNameInput.GetAttribute("value"))
            && _jobTitleDropdown.Displayed
            && _jobTitleDropdown.Text.Trim() != "-- Select --"
            && _hiringManagerInput.Displayed
            && !string.IsNullOrWhiteSpace(
                _hiringManagerInput.GetAttribute("value")));
    }

    public string GetVacancyName()
    {
        return _vacancyNameInput.GetAttribute("value") ?? string.Empty;
    }

    public string GetJobTitle()
    {
        return _jobTitleDropdown.Text.Trim();
    }

    public string GetHiringManager()
    {
        return _hiringManagerInput.GetAttribute("value") ?? string.Empty;
    }
}