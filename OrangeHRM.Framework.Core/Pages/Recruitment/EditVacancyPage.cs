using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class EditVacancyPage : LoadablePage
{
    private static readonly By EditVacancyHeader = By.XPath("//h6[normalize-space()='Edit Vacancy']");

    private readonly TextBox _vacancyNameInput = new(
        By.XPath("//label[normalize-space()='Vacancy Name']" +
                 "/ancestor::div[contains(@class,'oxd-input-group')]//input"));

    private readonly Label _jobTitleDropdown = new(
        By.XPath("//label[normalize-space()='Job Title']" +
                 "/ancestor::div[contains(@class,'oxd-input-group')]" +
                 "//div[contains(@class,'oxd-select-text-input')]"));

    private readonly TextBox _hiringManagerInput = new(
        By.XPath("//label[normalize-space()='Hiring Manager']" +
                 "/ancestor::div[contains(@class,'oxd-input-group')]//input"));

    public EditVacancyPage() : base(EditVacancyHeader)
    {
        Wait.Until(driver =>
            _vacancyNameInput.IsDisplayed()
            && !string.IsNullOrWhiteSpace(
                _vacancyNameInput.GetAttribute("value"))
            && _jobTitleDropdown.IsDisplayed()
            && _jobTitleDropdown.GetText().Trim() != "-- Select --"
            && _hiringManagerInput.IsDisplayed()
            && !string.IsNullOrWhiteSpace(
                _hiringManagerInput.GetAttribute("value")));
    }

    public string GetVacancyName()
    {
        return _vacancyNameInput.GetAttribute("value") ?? string.Empty;
    }

    public string GetJobTitle()
    {
        return _jobTitleDropdown.GetText().Trim();
    }

    public string GetHiringManager()
    {
        return _hiringManagerInput.GetAttribute("value") ?? string.Empty;
    }
}