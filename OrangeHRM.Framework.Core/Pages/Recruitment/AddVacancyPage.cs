using OpenQA.Selenium;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class AddVacancyPage : LoadablePage
{
    private static readonly By AddVacancyHeader = By.XPath("//h6[text()='Add Vacancy']");
    private static readonly By VacancyName =
        By.XPath("//label[normalize-space()='Vacancy Name']/ancestor::div[contains(@class,'oxd-input-group')]//input");

    private static readonly By JobTitleDropdown =
        By.XPath(
            "//label[normalize-space()='Job Title']" +
            "/ancestor::div[contains(@class,'oxd-input-group')]" +
            "//div[contains(@class,'oxd-select-text')]");
    
    private static readonly By HiringManagerInput =
        By.XPath(
            "//label[normalize-space()='Hiring Manager']" +
            "/ancestor::div[contains(@class,'oxd-input-group')]" +
            "//input[@placeholder='Type for hints...']");
    
    private static readonly By EditVacancyHeader =
        By.XPath("//h6[normalize-space()='Edit Vacancy']");

    private static readonly By ValidationErrors =
        By.CssSelector(".oxd-input-field-error-message");

    private static readonly By Alert =
        By.CssSelector(".oxd-alert-content-text");

    private static readonly By SaveButton = By.CssSelector("[type='submit']");
    
    public AddVacancyPage() : base(AddVacancyHeader)
    {
    }

    public void EnterVacancyName(string vacancyName)
    {
        TypeText(VacancyName, vacancyName);
    }

    public void SelectJobTitle(string jobTitle)
    {
        Click(JobTitleDropdown);

        var option = By.XPath(
            "//div[contains(@class,'oxd-select-option')]" +
            $"[normalize-space()={ToXPathLiteral(jobTitle)}]");

        Wait.Until(driver =>
            driver.FindElements(option).Any(element => element.Displayed));

        Click(option);
    }
    public void SelectHiringManager(string managerName)
    {
        TypeText(HiringManagerInput, managerName);

        var managerOption = By.XPath(
            $"//div[contains(@class,'oxd-autocomplete-option') and normalize-space()='{managerName}']");

        Wait.Until(driver =>
            driver.FindElements(managerOption)
                .Any(element => element.Displayed));

        Click(managerOption);
    }
    public void Save()
    {
        Click(SaveButton);

        Wait.Until(driver =>
            driver.FindElements(EditVacancyHeader)
                .Any(element => element.Displayed)
            ||
            driver.FindElements(ValidationErrors)
                .Any(element => element.Displayed)
            ||
            driver.FindElements(Alert)
                .Any(element => element.Displayed));
    }

    private static string ToXPathLiteral(string value)
    {
        if (!value.Contains('\''))
        {
            return $"'{value}'";
        }

        if (!value.Contains('"'))
        {
            return $"\"{value}\"";
        }

        var parts = value.Split('\'');
        return "concat('" +
               string.Join("', \"'\", '", parts) +
               "')";
    }
}