using OpenQA.Selenium;

namespace OrangeHRM.Framework.UI.Pages.Employees;

public class AddEmployeePage : LoadablePage
{
    private static readonly By AddEmployeeHeader = By.XPath("//h6[text()='Add Employee']");
    private static readonly By FirstNameInput = By.Name("firstName");
    private static readonly By MiddleNameInput = By.Name("middleName");
    private static readonly By LastNameInput = By.Name("lastName");
    private static readonly By SaveButton = By.CssSelector("button[type='submit']");
    private static readonly By PersonalDetailsHeader = By.XPath("//h6[text()='Personal Details']");
   
    public AddEmployeePage() : base(AddEmployeeHeader)
    {
    }
    
    public void CreateEmployee(
        string firstName,
        string lastName,
        string middleName = "")
    {
        TypeText(FirstNameInput, firstName);
        TypeText(MiddleNameInput, middleName);
        TypeText(LastNameInput, lastName);
        Click(SaveButton);
    }

    public bool IsEmployeeProfileDisplayed()
    {
        return FindVisibleElement(PersonalDetailsHeader).Displayed;
    }
   }