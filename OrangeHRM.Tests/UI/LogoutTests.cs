using OrangeHRM.Framework.Core.Pages;
using OrangeHRM.Framework.Core.Pages.Authentication;
using OrangeHRM.Tests.Configuration;
using OrangeHRM.Tests.Infrastructure;

namespace OrangeHRM.Tests.UI;

[TestFixture]
public class LogoutTests : UiTestBase
{
    [SetUp]
    public void SetUp()
    {
        var loginPage = new LoginPage();
        loginPage.Login(TestSettings.Username, TestSettings.Password);
    }
    
    [Test]
    public void Logout()
    {
        var loginPage = new DashboardPage()
            .OpenNavigationMenu()
            .Logout();

        Assert.That(loginPage.IsDisplayed(), Is.True,
            "Login page should be displayed.");
    }
}