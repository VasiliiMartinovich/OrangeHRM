using OpenQA.Selenium;

namespace OrangeHRM.Framework.Core.Pages;

public abstract class LoadablePage : BasePage
{
    protected LoadablePage(By pageReadyLocator)
    {
        FindVisibleElement(pageReadyLocator);
    }
}