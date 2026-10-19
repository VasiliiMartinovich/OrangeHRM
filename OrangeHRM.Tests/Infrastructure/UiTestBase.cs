using System.Text;
using Allure.Net.Commons;
using Allure.NUnit;
using log4net;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OrangeHRM.Framework.Core.Drivers;
using OrangeHRM.Tests.Configuration;

namespace OrangeHRM.Tests.Infrastructure;

[AllureNUnit]
public abstract class UiTestBase
{
    protected ILog Log => LogManager.GetLogger(GetType());

    [SetUp]
    public void SetUp()
    {
        Log4NetConfig.Configure();
        Log.Info("Starting UI test setup.");

        AllureApi.Step("Configure WebDriver timeout", () =>
        {
            DriverManager.TimeoutSeconds = TestSettings.TimeoutSeconds;
        });

        var browserType = Enum.Parse<BrowserType>(
            TestSettings.Browser,
            ignoreCase: true);

        var driver = AllureApi.Step(
            $"Create WebDriver: {browserType}",
            () => DriverFactory.CreateDriver(
                browserType,
                TestSettings.Headless));

        AllureApi.Step("Register WebDriver instance", () =>
        {
            DriverManager.SetDriver(driver);
        });

        AllureApi.Step("Set browser window size", () =>
        {
            DriverManager.Driver.Manage().Window.Size =
                new System.Drawing.Size(1920, 1080);
        });

        AllureApi.Step("Open application URL", () =>
        {
            DriverManager.Driver.Navigate().GoToUrl(
                TestSettings.BaseUrl);
        });

        Log.Info("Browser opened and application URL loaded.");
    }

    [TearDown]
    public void TearDown()
    {
        var result = TestContext.CurrentContext.Result;
        var testName = TestContext.CurrentContext.Test.Name;

        try
        {
            if (result.Outcome.Status == TestStatus.Failed)
            {
                Log.Error(
                    $"UI test failed: {testName}. " +
                    $"Message: {result.Message}");

                AttachFailureArtifacts();
            }
            else
            {
                Log.Info(
                    $"UI test finished: {testName}. " +
                    $"Status: {result.Outcome.Status}");
            }
        }
        finally
        {
            DriverManager.QuitDriver();
            Log.Info($"WebDriver closed for test: {testName}");
        }
    }

    private void AttachFailureArtifacts()
    {
        IWebDriver driver;

        try
        {
            driver = DriverManager.Driver;
        }
        catch (Exception exception)
        {
            Log.Error(
                "Could not access WebDriver to capture failure artifacts.",
                exception);

            return;
        }

        TryAttachFailureArtifact(
            "Failure screenshot",
            "image/png",
            () => ((ITakesScreenshot)driver)
                .GetScreenshot()
                .AsByteArray);

        TryAttachFailureArtifact(
            "Failure page source",
            "text/html",
            () => Encoding.UTF8.GetBytes(driver.PageSource));

        TryAttachFailureArtifact(
            "Browser URL at failure",
            "text/plain",
            () => Encoding.UTF8.GetBytes(driver.Url));
    }

    private void TryAttachFailureArtifact(
        string name,
        string contentType,
        Func<byte[]> contentFactory)
    {
        try
        {
            var content = contentFactory();

            AllureApi.AddAttachment(
                name,
                contentType,
                content);

            Log.Info($"Attached to Allure: {name}.");
        }
        catch (Exception exception)
        {
            // Ошибка создания вложения не должна скрывать исходную ошибку теста.
            Log.Error(
                $"Could not create or attach Allure artifact: {name}.",
                exception);
        }
    }
}