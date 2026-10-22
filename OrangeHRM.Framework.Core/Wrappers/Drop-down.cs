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
            $"and normalize-space()={ToXPathLiteral(optionText)}]");

        var option = new BaseElement(optionLocator);
        option.Click();
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