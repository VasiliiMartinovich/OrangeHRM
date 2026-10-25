using OrangeHRM.Framework.Core.Pages;
using OrangeHRM.Framework.Core.Pages.Authentication;
using OrangeHRM.Tests.Configuration;
using Reqnroll;

namespace OrangeHRM.Tests.StepDefinitions.Authentication;

[Binding]
public class LoginSteps
{
    private LoginPage _loginPage = null!;
    private DashboardPage _dashboardPage = null!;

    [Given("the OrangeHRM login page is displayed")]
    public void GivenTheOrangeHrmloginPageIsDisplayed()
    {
        _loginPage = new LoginPage();

        Assert.That(_loginPage.IsDisplayed(), Is.True,
            "Login page should be displayed.");
    }

    [When("I log in with valid credentials")]
    public void WhenILogInWithValidCredentials()
    {
        _dashboardPage = _loginPage.Login(TestSettings.Username, TestSettings.Password);
    }

    [Then("the Dashboard page should be displayed")]
    public void ThenTheDashboardPageShouldBeDisplayed()
    {
        Assert.That(_dashboardPage.IsDisplayed(), Is.True,
            "Dashboard should be displayed after successful login.");
    }

    [When("I log in with invalid credentials")]
    public void WhenILogInWithInvalidCredentials()
    {
        _loginPage
            .EnterUsername("test")
            .EnterPassword("testqa")
            .ClickLoginButton();
    }

    [Then("the login error message should be {string}")]
    public void ThenTheLoginErrorMessageShouldBe(string expectedMessage)
    {
        Assert.That(_loginPage.IsErrorMessageDisplayed(), Is.True,
            "An error message should be displayed.");

        Assert.That(_loginPage.GetErrorMessage(), Is.EqualTo(expectedMessage),
            "The login error message does not match.");
    }
}