using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Pages;
using OrangeHRM.Framework.Core.Wrappers;

namespace OrangeHRM.Framework.Core.Pages.Authentication;

public class LoginPage : LoadablePage
{
    private static readonly By UsernameField = By.Name("username");

    private readonly TextBox _usernameField =
        new(By.Name("username"));

    private readonly TextBox _passwordField =
        new(By.Name("password"));

    private readonly Button _loginButton =
        new(By.CssSelector("button[type='submit']"));

    private readonly Label _errorMessage =
        new(By.CssSelector(".oxd-alert-content"));

    public LoginPage() : base(UsernameField)
    {
    }

    public LoginPage EnterUsername(string username)
    {
        _usernameField.SetValue(username);
        return this;
    }

    public LoginPage EnterPassword(string password)
    {
        _passwordField.SetValue(password);
        return this;
    }

    public LoginPage ClickLoginButton()
    {
        _loginButton.Click();
        return this;
    }

    public bool IsDisplayed()
    {
        return _usernameField.IsDisplayed()
               && _passwordField.IsDisplayed();
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
        try
        {
            return Wait.Until(driver =>
                driver.FindElements(By.CssSelector(".oxd-alert-content"))
                    .Any(element => element.Displayed));
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public string GetErrorMessage()
    {
        return _errorMessage.GetText();
    }
}