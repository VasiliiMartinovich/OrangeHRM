using OpenQA.Selenium;

namespace OrangeHRM.Framework.UI.Pages;

public abstract class LoadablePage : BasePage
{
    protected LoadablePage(By pageReadyLocator)
    {
        FindVisibleElement(pageReadyLocator);
    }
}