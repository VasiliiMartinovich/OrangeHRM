using OrangeHRM.Framework.UI.Pages;
using OrangeHRM.Framework.UI.Pages.Authentication;
using OrangeHRM.Tests.Infrastructure;

namespace OrangeHRM.Tests.UI;

[TestFixture]
public class LogoutTests : UiTestBase
{
    [SetUp]
    public void SetUp()
    {
        var loginPage = new LoginPage();
        loginPage.Login("Admin", "admin123");
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