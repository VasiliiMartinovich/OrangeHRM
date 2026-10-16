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

    public AddEmployeePage() : base(AddEmployeeHeader)
    {
    }

    public void CreateEmployee(
        string firstName,
        string lastName,
        string middleName = "")
    {
        _firstName.SetValue(firstName);
        _middleName.SetValue(middleName);
        _lastName.SetValue(lastName);
        _saveButton.Click();
    }
}