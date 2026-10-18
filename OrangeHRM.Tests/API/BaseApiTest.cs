using RestSharp;

namespace OrangeHRM.Tests.API;

public class BaseApiTest
{
    protected RestClient Client;
    protected string AccessToken { get; private set; } = string.Empty;

    [SetUp]
    public void SetUp()
    {
        var options = new RestClientOptions(
            "https://opensource-demo.orangehrmlive.com"
        );

        Client = new RestClient(options);
        
        AccessToken = Environment.GetEnvironmentVariable("ORANGEHRM_ACCESS_TOKEN") ?? string.Empty;

        Assert.That(AccessToken, Is.Not.Empty,
            "Environment variable ORANGEHRM_ACCESS_TOKEN is not set.");
    }

    [TearDown]
    public void TearDown()
    {
        Client.Dispose();
    }
}