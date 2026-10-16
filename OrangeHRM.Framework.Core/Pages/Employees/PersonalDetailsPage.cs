using OpenQA.Selenium;

namespace OrangeHRM.Framework.Core.Pages.Employees;

public class PersonalDetailsPage : LoadablePage
{
    private static readonly By PersonalDetailsHeader = By.XPath("//h6[text()='Personal Details']");
    private static readonly By FirstNameInput = By.Name("firstName");
    private static readonly By LastNameInput = By.Name("lastName");
    
    public PersonalDetailsPage() : base(PersonalDetailsHeader)
    {
    }

    public bool IsDisplayed()
    {
        return FindVisibleElement(PersonalDetailsHeader).Displayed;
    }

    public string GetFirstName()
    {
        return FindVisibleElement(FirstNameInput).GetAttribute("value") ?? string.Empty;
    }

    public string GetLastName()
    {
        return FindVisibleElement(LastNameInput).GetAttribute("value") ?? string.Empty;
    }
    
    public string WaitForFirstName()
    {
        return Wait.Until(driver =>
        {
            var element = driver.FindElement(FirstNameInput);
            var value = element.GetAttribute("value");

            return !string.IsNullOrWhiteSpace(value)
                ? value
                : null;
        })!;
    }
}