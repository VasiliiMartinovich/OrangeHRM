using Allure.Net.Commons;
using log4net;
using OrangeHRM.Framework.Core.Drivers;
using OrangeHRM.Tests.Configuration;
using OpenQA.Selenium;
using Reqnroll;

namespace OrangeHRM.Tests.Infrastructure;

[Binding]
public sealed class ReqnrollHooks
{
    private readonly ScenarioContext _scenarioContext;
    private readonly ILog _log = LogManager.GetLogger(typeof(ReqnrollHooks));

    public ReqnrollHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    
    [BeforeScenario]
    public void BeforeScenario()
    {
        IWebDriver? driver = null;
        bool driverRegistered = false;

        try
        {
            Log4NetConfig.Configure();

            _log.Info(
                $"Starting BDD scenario: {_scenarioContext.ScenarioInfo.Title}");

            DriverManager.TimeoutSeconds = TestSettings.TimeoutSeconds;

            var browserType = Enum.Parse<BrowserType>(
                TestSettings.Browser,
                ignoreCase: true);

            // Создаём браузер.
            driver = DriverFactory.CreateDriver(
                browserType,
                TestSettings.Headless);

            // Регистрируем его в DriverManager.
            DriverManager.SetDriver(driver);
            driverRegistered = true;

            // Настраиваем браузер.
            DriverManager.Driver.Manage().Window.Size =
                new System.Drawing.Size(1920, 1080);

            // Открываем приложение.
            DriverManager.Driver.Navigate().GoToUrl(
                TestSettings.BaseUrl);

            _log.Info(
                "BDD scenario browser started and application opened.");
        }
        catch (Exception setupException)
        {
            _log.Error(
                "Failed to initialize browser for BDD scenario.",
                setupException);

            try
            {
                if (driverRegistered)
                {
                    DriverManager.QuitDriver();
                }
                else
                {
                    driver?.Quit();
                }
            }
            catch (Exception cleanupException)
            {
                _log.Error(
                    "Could not close WebDriver after setup failure.",
                    cleanupException);
            }

            // Не скрываем исходную ошибку настройки.
            throw;
        }
    }


    [AfterScenario]
    public void AfterScenario()
    {
        try
        {
            var testError = _scenarioContext.TestError;

            if (testError is not null)
            {
                _log.Error($"BDD scenario failed: {_scenarioContext.ScenarioInfo.Title}", testError);

                TryAttachFailureArtifact(
                    "Failure screenshot",
                    "image/png",
                    () => ((ITakesScreenshot)DriverManager.Driver)
                        .GetScreenshot()
                        .AsByteArray);

                TryAttachFailureArtifact(
                    "Failure page source",
                    "text/html",
                    () => System.Text.Encoding.UTF8.GetBytes(
                        DriverManager.Driver.PageSource));

                TryAttachFailureArtifact(
                    "Browser URL at failure",
                    "text/plain",
                    () => System.Text.Encoding.UTF8.GetBytes(
                        DriverManager.Driver.Url));
            }
            else
            {
                _log.Info(
                    $"BDD scenario passed: {_scenarioContext.ScenarioInfo.Title}");
            }
        }
        finally
        {
            try
            {
                DriverManager.QuitDriver();
            }
            catch (Exception exception)
            {
                _log.Error("Could not close WebDriver after BDD scenario.", exception);
            }
        }
    }

    private void TryAttachFailureArtifact(
        string name,
        string contentType,
        Func<byte[]> contentFactory)
    {
        try
        {
            AllureApi.AddAttachment(
                name,
                contentType,
                contentFactory());

            _log.Info($"Attached to Allure: {name}.");
        }
        catch (Exception exception)
        {
            _log.Error($"Could not create or attach Allure artifact: {name}.", exception);
        }
    }
}