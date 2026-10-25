using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Recruitment;

public class AddCandidatePage : LoadablePage
{
    private static readonly By AddCandidateHeader = By.XPath("//h6[text()='Add Candidate']");
    private static readonly By VacancyOptions = By.XPath("//div[contains(@class,'oxd-select-option')]");
    private readonly TextBox _firstNameInput = new(By.Name("firstName"));
    private readonly TextBox _middleNameInput = new(By.Name("middleName"));
    private readonly TextBox _lastNameInput = new(By.Name("lastName"));
    private readonly Button _saveButton = new(By.CssSelector("button[type='submit']"));
    private readonly Dropdown _vacancyDropdown = new(By.XPath("//label[normalize-space()='Vacancy']/ancestor::div[contains(@class,'oxd-input-group')]//div[contains(@class,'oxd-select-text')]"));
    private readonly TextBox _emailInput = new(By.XPath("//label[normalize-space()='Email']/../following-sibling::div//input"));

    [FindsBy(How = How.XPath, Using = "//input[@type='file']")]
    private IWebElement _cvInput = null!;

    public AddCandidatePage() : base(AddCandidateHeader)
    {
        PageFactory.InitElements(Driver, this);
    }

    public void FillCandidateForm(string firstName, string middleName, string lastName, string email)
    {
        Wait.Until(driver => _firstNameInput.IsDisplayed() && _firstNameInput.IsEnabled());

        _firstNameInput.SetValue(firstName);
        _middleNameInput.SetValue(middleName);
        _lastNameInput.SetValue(lastName);
        _emailInput.SetValue(email);
    }
    
    public void SelectVacancy(string vacancyName)
    {
        Wait.Until(driver => _vacancyDropdown.IsDisplayed() && _vacancyDropdown.IsEnabled());

        _vacancyDropdown.SelectOption(vacancyName);
    }

    public void UploadCv(string cvFilePath)
    {
        _cvInput.SendKeys(cvFilePath);
    }

    public bool IsCvSelected(string fileName)
    {
        var value = _cvInput.GetAttribute("value");

        return !string.IsNullOrEmpty(value) && value.EndsWith(fileName, StringComparison.OrdinalIgnoreCase);
    }

    public CandidateDetailsPage Save()
    {
        Wait.Until(driver => _saveButton.IsDisplayed() && _saveButton.IsEnabled());

        _saveButton.Click();

        return new CandidateDetailsPage();
    }
}
