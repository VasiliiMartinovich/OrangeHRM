using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class AddVacancyPage : LoadablePage
{
    private static readonly By AddVacancyHeader = By.XPath("//h6[text()='Add Vacancy']");
    private static readonly By EditVacancyHeader = By.XPath("//h6[normalize-space()='Edit Vacancy']");
    private static readonly By ValidationErrors = By.CssSelector(".oxd-input-field-error-message");
    private static readonly By Alert = By.CssSelector(".oxd-alert-content-text");
    private readonly TextBox _vacancyNameInput = new(By.XPath("//label[normalize-space()='Vacancy Name']" +
                                                              "/ancestor::div[contains(@class,'oxd-input-group')]//input"));

    private readonly Dropdown _jobTitleDropdown = new(By.XPath("//label[normalize-space()='Job Title']" +
                                                               "/ancestor::div[contains(@class,'oxd-input-group')]" +
                                                               "//div[contains(@class,'oxd-select-text')]"));

    private readonly TextBox _hiringManagerInput = new(By.XPath("//label[normalize-space()='Hiring Manager']" +
                                                                "/ancestor::div[contains(@class,'oxd-input-group')]" +
                                                                "//input[@placeholder='Type for hints...']"));

    private readonly Button _saveButton = new(By.CssSelector("[type='submit']"));

    public AddVacancyPage() : base(AddVacancyHeader)
    {
    }

    public void EnterVacancyName(string vacancyName)
    {
        _vacancyNameInput.Clear();
        _vacancyNameInput.SetValue(vacancyName);
    }
   
    public void SelectJobTitle(string jobTitle)
    {
        _jobTitleDropdown.SelectOption(jobTitle);
    }

    public void SelectHiringManager(string managerName)
    {
        _hiringManagerInput.Clear();
        _hiringManagerInput.SetValue(managerName);

        var managerOption = By.XPath(
            "//div[contains(@class,'oxd-autocomplete-option') and " +
            $"normalize-space()={ToXPathLiteral(managerName)}]");

        Wait.Until(driver =>
            driver.FindElements(managerOption).Any(element => element.Displayed));

        Click(managerOption);
    }

    public void Save()
    {
        _saveButton.Click();

        Wait.Until(driver =>
            driver.FindElements(EditVacancyHeader).Any(element => element.Displayed)
            ||
            driver.FindElements(ValidationErrors).Any(element => element.Displayed)
            ||
            driver.FindElements(Alert).Any(element => element.Displayed));
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
               string.Join("', \"'\", '", parts) + "')";
    }
}
