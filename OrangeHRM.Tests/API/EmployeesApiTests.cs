using OrangeHRM.Framework.Core.Api.Clients;

namespace OrangeHRM.Tests.API;

[TestFixture]
public class EmployeesApiTests
{
    [Test]
    public void GetEmployees_WithAccessToken_ShouldReturnSuccess()
    {
        var accessToken = Environment.GetEnvironmentVariable(
            "ORANGEHRM_ACCESS_TOKEN");

        Assert.That(
            accessToken,
            Is.Not.Null.And.Not.Empty,
            "Environment variable ORANGEHRM_ACCESS_TOKEN is not set.");

        using var client = new EmployeesApiClient(accessToken!);

        var response = client.GetEmployees();

        TestContext.WriteLine(
            $"Status code: {(int)response.StatusCode}");

        TestContext.WriteLine(
            $"Response body: {response.Content}");

        Assert.That(
            response.ErrorException,
            Is.Null,
            "An exception occurred while sending the request.");

        Assert.That(
            (int)response.StatusCode,
            Is.EqualTo(200),
            "Expected HTTP 200 from the employees API.");
    }
}