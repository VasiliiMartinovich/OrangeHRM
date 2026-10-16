using OpenQA.Selenium;

namespace OrangeHRM.Framework.Core.Wrappers;

public class TextBox : BaseElement
{
    public TextBox(By locator) : base(locator)
    {
    }
    
    public string GetValue()
    {
        return GetAttribute("value") ?? string.Empty;
    }
}