using OpenQA.Selenium;

namespace OrangeHRM.Framework.Core.Wrappers;

public class Dropdown : BaseElement
{
    public Dropdown(By locator) : base(locator)
    {
    }

    public void SelectOption(string optionText)
    {
        Click();

        var optionLocator = By.XPath(
            $"//*[contains(@class,'oxd-select-option') " +
            $"and normalize-space()='{optionText}']");

        var option = new BaseElement(optionLocator);
        option.Click();
    }
}