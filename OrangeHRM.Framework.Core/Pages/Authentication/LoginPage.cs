using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace OrangeHRM.Framework.Core.Pages.Authentication;

public class LoginPage : LoadablePage
{
    private static readonly By UsernameLocator = By.Name("username");

    [FindsBy(How = How.Name, Using = "username")]
    private IWebElement _usernameField = null!;

    [FindsBy(How = How.Name, Using = "password")]
    private IWebElement _passwordField = null!;

    [FindsBy(How = How.CssSelector, Using = "button[type='submit']")]
    private IWebElement _loginButton = null!;

    [FindsBy(How = How.CssSelector, Using = ".oxd-alert-content")]
    private IWebElement _errorMessage = null!;

    public LoginPage() : base(UsernameLocator)
    {
        PageFactory.InitElements(Driver, this);
    }

    public LoginPage EnterUsername(string username)
    {
        _usernameField.Clear();
        _usernameField.SendKeys(username);
        return this;
    }

    public LoginPage EnterPassword(string password)
    {
        _passwordField.Clear();
        _passwordField.SendKeys(password);
        return this;
    }

    public LoginPage ClickLoginButton()
    {
        _loginButton.Click();
        return this;
    }

    public bool IsDisplayed()
    {
        return Wait.Until(driver => _usernameField.Displayed && _passwordField.Displayed);
    }

    public DashboardPage Login(string username, string password)
    {
        EnterUsername(username);
        EnterPassword(password);
        ClickLoginButton();

        return new DashboardPage();
    }

    public bool IsErrorMessageDisplayed()
    {
        return Wait.Until(driver =>
            driver.FindElements(By.CssSelector(".oxd-alert-content")).Any(element => element.Displayed));
    }

    public string GetErrorMessage()
    {
        return Wait.Until(driver =>
            driver.FindElements(By.CssSelector(".oxd-alert-content")).FirstOrDefault(element => element.Displayed)?.Text);
    }
}