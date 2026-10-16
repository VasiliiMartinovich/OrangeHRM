using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Employees;

public class PersonalDetailsPage : LoadablePage
{
    private static readonly By PersonalDetailsHeader = By.XPath("//h6[text()='Personal Details']");
    private readonly TextBox _firstNameInput = new(By.Name("firstName"));
    private readonly TextBox _lastNameInput = new(By.Name("lastName"));

    public PersonalDetailsPage() : base(PersonalDetailsHeader)
    {
    }

    public bool IsDisplayed()
    {
        return FindVisibleElement(PersonalDetailsHeader).Displayed;
    }

    public string GetFirstName()
    {
        return _firstNameInput.GetValue();
    }

    public string GetLastName()
    {
        return _lastNameInput.GetValue();
    }

    public string WaitForFirstName()
    {
        return Wait.Until(driver =>
        {
            var elements = driver.FindElements(
                By.Name("firstName"));

            foreach (var element in elements)
            {
                if (!element.Displayed)
                {
                    continue;
                }
                var value = element.GetAttribute("value");
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
            return null;
        })!;
    }
}