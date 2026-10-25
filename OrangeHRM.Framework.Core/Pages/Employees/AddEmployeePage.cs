using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Employees;

public class AddEmployeePage : LoadablePage
{
    private static readonly By AddEmployeeHeader = By.XPath("//h6[text()='Add Employee']");
    private readonly TextBox _firstName = new(By.Name("firstName"));
    private readonly TextBox _middleName = new(By.Name("middleName"));
    private readonly TextBox _lastName = new(By.Name("lastName"));
    private readonly Button _saveButton = new(By.CssSelector("button[type='submit']"));
    private readonly TextBox _employeeIdInput = new(By.XPath("//label[normalize-space()='Employee Id']/ancestor::div[contains(@class,'oxd-input-group')]//input"));
    
   public AddEmployeePage() : base(AddEmployeeHeader)
    {
    }

    public void CreateEmployee(string firstName, string lastName, string middleName = "", string employeeId = "")
    {
        _firstName.SetValue(firstName);
        _middleName.SetValue(middleName);
        _lastName.SetValue(lastName);

        if (!string.IsNullOrWhiteSpace(employeeId))
        {
            _employeeIdInput.SetValue(employeeId);
        }

        _saveButton.Click();

        Wait.Until(driver =>
        {
            var personalDetailsHeader = driver.FindElements(By.XPath("//h6[normalize-space()='Personal Details']"));

            if (personalDetailsHeader.Any(element => element.Displayed))
            {
                return true;
            }

            var validationErrors = driver.FindElements(By.CssSelector(".oxd-input-field-error-message"));

            var visibleErrors = validationErrors
                .Where(element => element.Displayed)
                .ToList();

            if (visibleErrors.Count > 0)
            {
                throw new InvalidOperationException(
                    "Employee creation failed: " +
                    string.Join(" | ", visibleErrors.Select(element => element.Text)));
            }

            return false;
        });
    }

    public void EnterEmployeeId(string employeeId)
    {
        _employeeIdInput.Click();
        _employeeIdInput.SetValue(employeeId);
    }
    
    public string GetEmployeeId()
    {
        return _employeeIdInput.GetValue();
    }
}