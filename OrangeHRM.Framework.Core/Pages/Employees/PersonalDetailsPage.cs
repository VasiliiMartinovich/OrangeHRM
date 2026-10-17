using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace OrangeHRM.Framework.Core.Pages.Employees;

public class PersonalDetailsPage : LoadablePage
{
    private static readonly By PersonalDetailsHeader = By.XPath("//h6[text()='Personal Details']");

    [FindsBy(How = How.Name, Using = "firstName")]
    private IWebElement _firstNameInput = null!;

    [FindsBy(How = How.Name, Using = "lastName")]
    private IWebElement _lastNameInput = null!;

    public PersonalDetailsPage() : base(PersonalDetailsHeader)
    {
        PageFactory.InitElements(Driver, this);
    }

    public bool IsDisplayed()
    {
        return FindVisibleElement(PersonalDetailsHeader).Displayed;
    }

    public string GetFirstName()
    {
        return _firstNameInput.GetAttribute("value") ?? string.Empty;
    }

    public string GetLastName()
    {
        return _lastNameInput.GetAttribute("value") ?? string.Empty;
    }

    public string WaitForFirstName()
    {
        return Wait.Until(driver =>
        {
            foreach (var element in driver.FindElements(By.Name("firstName")))
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