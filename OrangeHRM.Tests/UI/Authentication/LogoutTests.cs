using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using log4net;
using OrangeHRM.Framework.Core.Pages;
using OrangeHRM.Framework.Core.Pages.Authentication;
using OrangeHRM.Tests.Configuration;
using OrangeHRM.Tests.Infrastructure;

namespace OrangeHRM.Tests.UI.Authentication;

[TestFixture]
public class LogoutTests : UiTestBase
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(LogoutTests));

    [SetUp]
    public void SetUp()
    {
        Log.Info("LogoutTests setup: logging in to OrangeHRM.");

        var loginPage = AllureApi.Step(
            "Create LoginPage object",
            () => new LoginPage());

        AllureApi.Step(
            "Log in with configured valid credentials",
            () => loginPage.Login(
                TestSettings.Username,
                TestSettings.Password));

        Log.Info("LogoutTests setup: login completed.");
    }

    [Test]
    [AllureName("Logout - user returns to Login Page")]
    [AllureTag("UI",  "Smoke")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription("Verifies that an authenticated user can log out from the OrangeHRM and return to the Login Page.")]
    public void Logout()
    {
        Log.Info("Test started: Logout.");

        var dashboardPage = AllureApi.Step(
            "Create DashboardPage object",
            () => new DashboardPage());

        Log.Info("Opening the navigation menu.");

        AllureApi.Step("Open navigation menu", () => dashboardPage.OpenNavigationMenu());

        Log.Info("Logging out.");

        var loginPage = AllureApi.Step(
            "Log out from Dashboard",
            () => dashboardPage.Logout());

        Log.Info("Checking that the login page is displayed.");

        AllureApi.Step("Verify Login Page is displayed", () =>
        {
            Assert.That(loginPage.IsDisplayed(), Is.True,
                "Login page should be displayed.");
        });

        Log.Info("Test passed: logout returned the user to the login page.");
    }
}