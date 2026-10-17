
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class AddCandidatePage : LoadablePage
{
    private static readonly By AddCandidateHeader = By.XPath("//h6[text()='Add Candidate']");
    private static readonly By VacancyOptions = By.XPath("//div[contains(@class,'oxd-select-option')]");

    [FindsBy(How = How.Name, Using = "firstName")]
    private IWebElement _firstNameInput = null!;

    [FindsBy(How = How.Name, Using = "middleName")]
    private IWebElement _middleNameInput = null!;

    [FindsBy(How = How.Name, Using = "lastName")]
    private IWebElement _lastNameInput = null!;

    [FindsBy(
        How = How.XPath,
        Using = "//label[normalize-space()='Vacancy']/ancestor::div[contains(@class,'oxd-input-group')]//div[contains(@class,'oxd-select-text')]")]
    private IWebElement _vacancyDropdown = null!;

    [FindsBy(
        How = How.XPath,
        Using = "//label[normalize-space()='Email']/../following-sibling::div//input")]
    private IWebElement _emailInput = null!;

    [FindsBy(How = How.XPath, Using = "//input[@type='file']")]
    private IWebElement _cvInput = null!;

    [FindsBy(How = How.CssSelector, Using = "button[type='submit']")]
    private IWebElement _saveButton = null!;

    public AddCandidatePage() : base(AddCandidateHeader)
    {
        PageFactory.InitElements(Driver, this);
    }

    public void FillCandidateForm(
        string firstName,
        string middleName,
        string lastName,
        string email)
    {
        Wait.Until(driver =>
            _firstNameInput.Displayed && _firstNameInput.Enabled);

        _firstNameInput.Clear();
        _firstNameInput.SendKeys(firstName);

        _middleNameInput.Clear();
        _middleNameInput.SendKeys(middleName);

        _lastNameInput.Clear();
        _lastNameInput.SendKeys(lastName);

        _emailInput.Clear();
        _emailInput.SendKeys(email);
    }

    public void SelectVacancy(string vacancyName)
    {
        Wait.Until(driver =>
            _vacancyDropdown.Displayed && _vacancyDropdown.Enabled);

        _vacancyDropdown.Click();

        Wait.Until(driver => driver.FindElements(VacancyOptions).Any(element => element.Displayed));

        var visibleOptions = Driver.FindElements(VacancyOptions)
            .Where(element => element.Displayed)
            .Select(element => element.Text.Trim())
            .ToList();

       var vacancyOption = By.XPath(
            "//div[contains(@class,'oxd-select-option')]" +
            $"[normalize-space()={ToXPathLiteral(vacancyName)}]");

        Wait.Until(driver => driver.FindElements(vacancyOption).Any(element => element.Displayed));

        var option = Driver.FindElements(vacancyOption)
            .First(element => element.Displayed);

        option.Click();
    }

    public void UploadCv(string cvFilePath)
    {
        _cvInput.SendKeys(cvFilePath);
    }

    public bool IsCvSelected(string fileName)
    {
        var value = _cvInput.GetAttribute("value");

        return !string.IsNullOrEmpty(value) &&
               value.EndsWith(
                   fileName,
                   StringComparison.OrdinalIgnoreCase);
    }

    public CandidateDetailsPage Save()
    {
        Wait.Until(driver =>
            _saveButton.Displayed && _saveButton.Enabled);

        _saveButton.Click();

        return new CandidateDetailsPage();
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
