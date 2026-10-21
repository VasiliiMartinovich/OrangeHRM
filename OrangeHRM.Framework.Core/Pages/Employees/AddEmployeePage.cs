using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace OrangeHRM.Framework.Core.Pages.Employees;

public class AddEmployeePage : LoadablePage
{
    private static readonly By AddEmployeeHeader = By.XPath("//h6[text()='Add Employee']");

    [FindsBy(How = How.Name, Using = "firstName")]
    private IWebElement _firstName = null!;

    [FindsBy(How = How.Name, Using = "middleName")]
    private IWebElement _middleName = null!;

    [FindsBy(How = How.Name, Using = "lastName")]
    private IWebElement _lastName = null!;

    [FindsBy(How = How.CssSelector, Using = "button[type='submit']")]
    private IWebElement _saveButton = null!;
    
    [FindsBy(
        How = How.XPath,
        Using = "//label[normalize-space()='Employee Id']/ancestor::div[contains(@class,'oxd-input-group')]//input")]
    private IWebElement _employeeIdInput = null!;

    public AddEmployeePage() : base(AddEmployeeHeader)
    {
        PageFactory.InitElements(Driver, this);
    }

    public void CreateEmployee(
        string firstName,
        string lastName,
        string middleName = "",
        string employeeId = "")
    {
        Wait.Until(driver =>
            _firstName.Displayed && _firstName.Enabled);

        _firstName.Clear();
        _firstName.SendKeys(firstName);

        _middleName.Clear();
        _middleName.SendKeys(middleName);

        _lastName.Clear();
        _lastName.SendKeys(lastName);

        if (!string.IsNullOrWhiteSpace(employeeId))
        {
            EnterEmployeeId(employeeId);
        }

        Wait.Until(driver =>
            _saveButton.Displayed && _saveButton.Enabled);

        _saveButton.Click();
    }
    
    public void EnterEmployeeId(string employeeId)
    {
        _employeeIdInput.Click();
        _employeeIdInput.SendKeys(Keys.Control + "a");
        _employeeIdInput.SendKeys(Keys.Backspace);
        _employeeIdInput.SendKeys(employeeId);
    }
    
    public string GetEmployeeId()
    {
        Wait.Until(driver => _employeeIdInput.Displayed);
        return _employeeIdInput.GetAttribute("value") ?? string.Empty;
    }
}