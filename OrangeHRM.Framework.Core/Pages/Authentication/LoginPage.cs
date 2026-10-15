using OpenQA.Selenium;

namespace OrangeHRM.Framework.UI.Pages.Authentication;

public class LoginPage : LoadablePage
{
    private static readonly By UsernameField = By.Name("username");
    private readonly By _passwordField = By.Name("password");
    private readonly By _loginButton = By.CssSelector("button[type='submit']");
    private readonly By _errorMessage = By.CssSelector(".oxd-alert-content");

    public LoginPage() : base(UsernameField)
    {
    }

    public LoginPage EnterUsername(string username)
    {
        TypeText(UsernameField, username);
        return this;
    }

    public LoginPage EnterPassword(string password)
    {
        TypeText(_passwordField, password);
        return this;
    }

    public LoginPage ClickLoginButton()
    {
        Click(_loginButton);
        return this;
    }
    
    public bool IsDisplayed()
    {
        return FindVisibleElement(UsernameField).Displayed
               && FindVisibleElement(_passwordField).Displayed;
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
        return FindVisibleElement(_errorMessage).Displayed;
    }

    public string GetErrorMessage()
    {
        return FindVisibleElement(_errorMessage).Text;
    }   
}