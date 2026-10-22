using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using log4net;
using OrangeHRM.Framework.Core.Pages.Authentication;
using OrangeHRM.Tests.Infrastructure;
using OrangeHRM.Tests.Configuration;

namespace OrangeHRM.Tests.UI.Authentication;

[TestFixture]
[Parallelizable(ParallelScope.Children)]

public class LoginPageTests : UiTestBase
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(LoginPageTests));

    [Test]
    [AllureName("Login Page - page is displayed")]
    [AllureTag("UI", "Smoke")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Verifies that the OrangeHRM login page is displayed when the application is opened.")]
    public void LoginPage_ShouldBeDisplayed()
    {
        Log.Info("Test started: LoginPage_ShouldBeDisplayed.");

        var loginPage = AllureApi.Step(
            "Create LoginPage object",
            () => new LoginPage());

        Log.Info("Checking that the login page is displayed.");

        AllureApi.Step("Verify login page is displayed", () =>
        {
            Assert.That(loginPage.IsDisplayed(), Is.True,
                "Login page should be displayed.");
        });

        Log.Info("Test passed: login page is displayed.");
    }

    [Test]
    [AllureName("Login - successful login opens Dashboard")]
    [AllureTag("UI", "Smoke")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription(
        "Verifies that a user can log in using the configured valid credentials and is redirected to the Dashboard page.")]
    public void SuccessLogin()
    {
        Log.Info("Test started: SuccessLogin.");

        var loginPage = AllureApi.Step(
            "Create LoginPage object",
            () => new LoginPage());

        Log.Info("Attempting to log in with configured valid credentials.");

        var dashboardPage = AllureApi.Step(
            "Log in with configured valid credentials",
            () => loginPage.Login(
                TestSettings.Username,
                TestSettings.Password));

        Log.Info("Login action completed. Checking that the dashboard is displayed.");

        AllureApi.Step("Verify Dashboard is displayed", () =>
        {
            Assert.That(dashboardPage.IsDisplayed(), Is.True,
                "Dashboard should be displayed after successful login.");
        });

        Log.Info("Test passed: successful login opened the dashboard.");
    }

    [Test]
    [AllureName("Login - invalid credentials are rejected")]
    [AllureTag("UI", "Validation")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Verifies that submitting invalid login credentials displays the error message 'Invalid credentials'.")]
    public void LoginWithInvalidCredentials()
    {
        Log.Info("Test started: LoginWithInvalidCredentials.");

        var loginPage = AllureApi.Step(
            "Create LoginPage object",
            () => new LoginPage());

        Log.Info("Entering invalid login credentials.");

        AllureApi.Step("Enter invalid credentials and submit login form", () =>
        {
            loginPage
                .EnterUsername(username: "test")
                .EnterPassword(password: "test124")
                .ClickLoginButton();
        });

        Log.Info("Checking that the login error message is displayed.");

        AllureApi.Step("Verify login error message is displayed", () =>
        {
            Assert.That(loginPage.IsErrorMessageDisplayed(), Is.True,
                "An error message should be displayed for invalid credentials.");
        });

        Log.Info("Reading the login error message.");

        var actualErrorMessage = AllureApi.Step(
            "Read login error message",
            () => loginPage.GetErrorMessage());

        Log.Info($"Login error message received: {actualErrorMessage}");

        AllureApi.Step("Verify login error message text", () =>
        {
            Assert.That(actualErrorMessage, Is.EqualTo("Invalid credentials"),
                "The error message should indicate invalid credentials.");
        });

        Log.Info("Test passed: invalid credentials were rejected.");
    }

    [Test]
    [AllureName("Configuration - application settings are loaded")]
    [AllureTag("UI", "Configuration")]
    [AllureSeverity(SeverityLevel.minor)]
    [AllureDescription(
        "Verifies that the application URL, browser name, and timeout match the expected test configuration.")]
    public void Configuration_ShouldBeLoaded()
    {
        Log.Info("Test started: Configuration_ShouldBeLoaded.");

        Log.Info("Checking the configured application URL.");

        AllureApi.Step("Verify application URL", () =>
        {
            Assert.That(TestSettings.BaseUrl, Is.EqualTo("https://opensource-demo.orangehrmlive.com/"));
        });
        
        Log.Info("Checking the configured timeout.");

        AllureApi.Step("Verify configured timeout", () =>
        {
            Assert.That(TestSettings.TimeoutSeconds, Is.EqualTo(10));
        });

        Log.Info("Test passed: application configuration values are correct.");
    }
}