using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OrangeHRM.Framework.Core.Drivers;
using OrangeHRM.Framework.Core.Pages;
using OrangeHRM.Framework.Core.Pages.Authentication;
using OrangeHRM.Tests.Infrastructure;
using OrangeHRM.Tests.Configuration;

namespace OrangeHRM.Tests.UI.Authentication;

[TestFixture]
public class LoginPageTests : UiTestBase
{
    [Test]
    public void LoginPage_ShouldBeDisplayed()
    {
        var loginPage = new LoginPage();
        Assert.That(loginPage.IsDisplayed(), Is.True);
    }
    
    [Test]
    public void SuccessLogin()
    {
        var loginPage = new LoginPage();
        var dashboardPage = loginPage.Login(
            TestSettings.Username,
            TestSettings.Password);
        Assert.That(dashboardPage.IsDisplayed(), Is.True);
    }
    
    [Test]
    public void LoginWithInvalidCredentials()
    {
        var loginPage = new LoginPage();
        loginPage.EnterUsername(username: "test").EnterPassword(password: "test124").ClickLoginButton();
        Assert.That(loginPage.IsErrorMessageDisplayed(), Is.True);
        Assert.That(loginPage.GetErrorMessage(), Is.EqualTo("Invalid credentials"));
    }
    
    [Test]
    public void Configuration_ShouldBeLoaded()
    {
        Assert.That(TestSettings.BaseUrl,
            Is.EqualTo("https://opensource-demo.orangehrmlive.com/"));

        Assert.That(TestSettings.Browser, Is.EqualTo("Chrome"));
        Assert.That(TestSettings.TimeoutSeconds, Is.EqualTo(10));
    }
}