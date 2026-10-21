using Allure.NUnit;
using log4net;
using OrangeHRM.Tests.Infrastructure;
using RestSharp;

namespace OrangeHRM.Tests.API;

[AllureNUnit]
public class BaseApiTest
{
    protected RestClient? Client;
    protected string AccessToken { get; private set; } = string.Empty;

    protected ILog Log => LogManager.GetLogger(GetType());

    [SetUp]
    public void SetUp()
    {
        Log4NetConfig.Configure();

        Log.Info($"Starting API test: {TestContext.CurrentContext.Test.Name}");

        var options = new RestClientOptions("https://opensource-demo.orangehrmlive.com");

        Client = new RestClient(options);

        AccessToken = Environment.GetEnvironmentVariable("ORANGEHRM_ACCESS_TOKEN") ?? string.Empty;

        Assert.That(AccessToken, Is.Not.Empty,
            "Environment variable ORANGEHRM_ACCESS_TOKEN is not set.");

        Log.Info("API test setup completed.");
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            Client?.Dispose();

            Log.Info(
                $"API test finished: {TestContext.CurrentContext.Test.Name}. " +
                $"Status: {TestContext.CurrentContext.Result.Outcome.Status}");
        }
        catch (Exception exception)
        {
            Log.Error("Failed to clean up API test resources.", exception);
            throw;
        }
    }
}