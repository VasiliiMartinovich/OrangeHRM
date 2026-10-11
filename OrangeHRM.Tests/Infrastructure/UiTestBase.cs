using NUnit.Framework;
using OrangeHRM.Framework.Core.Drivers;
using OrangeHRM.Tests.Configuration;

namespace OrangeHRM.Tests.Infrastructure;

public abstract class UiTestBase
{
    [SetUp]
    public void SetUp()
    {
        DriverManager.TimeoutSeconds = TestSettings.TimeoutSeconds;
        
        var browserType = Enum.Parse<BrowserType>(TestSettings.Browser, ignoreCase: true);

        var driver = DriverFactory.CreateDriver(browserType, TestSettings.Headless);

        DriverManager.SetDriver(driver);

        DriverManager.Driver.Manage().Window.Size =
            new System.Drawing.Size(1920, 1080);

        DriverManager.Driver.Navigate().GoToUrl(
            TestSettings.BaseUrl);
    }

    [TearDown]
    public void TearDown()
    {
        DriverManager.QuitDriver();
    }
}