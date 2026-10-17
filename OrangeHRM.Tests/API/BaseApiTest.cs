using RestSharp;

namespace OrangeHRM.Tests.API;

public class BaseApiTest
{
    protected RestClient Client;

    [SetUp]
    public void SetUp()
    {
        var options = new RestClientOptions(
            "https://opensource-demo.orangehrmlive.com"
        );

        Client = new RestClient(options);
    }

    [TearDown]
    public void TearDown()
    {
        Client.Dispose();
    }
}