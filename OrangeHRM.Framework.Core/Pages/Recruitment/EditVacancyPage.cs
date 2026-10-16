using OpenQA.Selenium;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class EditVacancyPage : LoadablePage
{
    private static readonly By EditVacancyHeader = By.XPath("//h6[normalize-space()='Edit Vacancy']");
    private static readonly By VacancyNameInput =
        By.XPath("//label[normalize-space()='Vacancy Name']/ancestor::div[contains(@class,'oxd-input-group')]//input");

    private static readonly By JobTitleDropdown =
        By.XPath("//label[normalize-space()='Job Title']/ancestor::div[contains(@class,'oxd-input-group')]//div[contains(@class,'oxd-select-text-input')]");

    private static readonly By HiringManagerInput =
        By.XPath("//label[normalize-space()='Hiring Manager']/ancestor::div[contains(@class,'oxd-input-group')]//input");

    public EditVacancyPage() : base(EditVacancyHeader)
    {
        Wait.Until(driver =>
        {
            var vacancyName = driver.FindElements(VacancyNameInput)
                .FirstOrDefault(element => element.Displayed);

            var jobTitle = driver.FindElements(JobTitleDropdown)
                .FirstOrDefault(element => element.Displayed);

            var hiringManager = driver.FindElements(HiringManagerInput)
                .FirstOrDefault(element => element.Displayed);

            return vacancyName != null
                   && !string.IsNullOrWhiteSpace(
                       vacancyName.GetAttribute("value"))
                   && jobTitle != null
                   && jobTitle.Text.Trim() != "-- Select --"
                   && hiringManager != null
                   && !string.IsNullOrWhiteSpace(
                       hiringManager.GetAttribute("value"));
        });
    }

    public string GetVacancyName()
    {
        var element = FindVisibleElement(VacancyNameInput);
        return element.GetAttribute("value") ?? string.Empty;
    }

    public string GetJobTitle()
    {
        return FindVisibleElement(JobTitleDropdown).Text.Trim();
    }

    public string GetHiringManager()
    {
        return FindVisibleElement(HiringManagerInput)
            .GetAttribute("value") ?? string.Empty;
    }
}