using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class AddVacancyPage : LoadablePage
{
    private static readonly By AddVacancyHeader = By.XPath("//h6[text()='Add Vacancy']");
    private static readonly By EditVacancyHeader = By.XPath("//h6[normalize-space()='Edit Vacancy']");
    private static readonly By ValidationErrors = By.CssSelector(".oxd-input-field-error-message");
    private static readonly By Alert = By.CssSelector(".oxd-alert-content-text");

    [FindsBy(
        How = How.XPath,
        Using = "//label[normalize-space()='Vacancy Name']/ancestor::div[contains(@class,'oxd-input-group')]//input")]
    private IWebElement _vacancyNameInput = null!;

    [FindsBy(
        How = How.XPath,
        Using = "//label[normalize-space()='Job Title']/ancestor::div[contains(@class,'oxd-input-group')]//div[contains(@class,'oxd-select-text')]")]
    private IWebElement _jobTitleDropdown = null!;

    [FindsBy(
        How = How.XPath,
        Using = "//label[normalize-space()='Hiring Manager']/ancestor::div[contains(@class,'oxd-input-group')]//input[@placeholder='Type for hints...']")]
    private IWebElement _hiringManagerInput = null!;

    [FindsBy(How = How.CssSelector, Using = "[type='submit']")]
    private IWebElement _saveButton = null!;

    public AddVacancyPage() : base(AddVacancyHeader)
    {
        PageFactory.InitElements(Driver, this);
    }

    public void EnterVacancyName(string vacancyName)
    {
        _vacancyNameInput.Clear();
        _vacancyNameInput.SendKeys(vacancyName);
    }

    public void SelectJobTitle(string jobTitle)
    {
        _jobTitleDropdown.Click();

        var option = By.XPath(
            "//div[contains(@class,'oxd-select-option')]" +
            $"[normalize-space()={ToXPathLiteral(jobTitle)}]");

        Wait.Until(driver => driver.FindElements(option).Any(element => element.Displayed));
        Click(option);
    }

    public void SelectHiringManager(string managerName)
    {
        _hiringManagerInput.Clear();
        _hiringManagerInput.SendKeys(managerName);

        var managerOption = By.XPath(
            "//div[contains(@class,'oxd-autocomplete-option') and " +
            $"normalize-space()={ToXPathLiteral(managerName)}]");

        Wait.Until(driver =>
            driver.FindElements(managerOption).Any(element => element.Displayed)); Click(managerOption);
    }

    public void Save()
    {
        _saveButton.Click();

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
