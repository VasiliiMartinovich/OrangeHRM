using System.Text.Json;
using OrangeHRM.Framework.Core.Api.Clients;
using OrangeHRM.Framework.Core.Api.Models.Requests;
using OrangeHRM.Framework.Core.Api.Models.Responses;

namespace OrangeHRM.Tests.API;

[TestFixture]
public class EmployeesApiTests : BaseApiTest
{
    [Test]
    public void GetEmployees_WithAccessToken_ShouldReturnSuccess()
    {
        using var client = new EmployeesApiClient(AccessToken!);

        var response = client.GetEmployees();

        Console.WriteLine($"Status code: {(int)response.StatusCode}");
        Console.WriteLine($"Response body: {response.Content}");

        Assert.That(response.ErrorException, Is.Null,
            "An exception occurred while sending the request.");

        Assert.That((int)response.StatusCode, Is.EqualTo(200),
            "Expected HTTP 200 from the employees API.");
    }
    
    [Test]
    public void CreateEmployee_ShouldReturnEmployeeId()
    {
        using var client = new EmployeesApiClient(AccessToken);

        var employeeRequest = new CreateEmployeeRequest
        {
            FirstName = $"Test{Guid.NewGuid().ToString("N")[..6]}",
            LastName = "ApiTest"
        };

        var response = client.CreateEmployee(employeeRequest);

        Console.WriteLine($"Status: {(int)response.StatusCode}");
        Console.WriteLine($"Body: {response.Content}");

        Assert.That(response.IsSuccessful, Is.True,
            $"Employee creation failed. " +
            $"Status: {(int)response.StatusCode}. " +
            $"Body: {response.Content}");

        var createdEmployee = JsonSerializer.Deserialize<CreateEmployeeResponse>(response.Content!);

        Assert.That(createdEmployee, Is.Not.Null);
        Assert.That(createdEmployee!.Data, Is.Not.Null);
        Assert.That(createdEmployee.Data!.EmpNumber, Is.GreaterThan(0));
        Assert.That(createdEmployee.Data.FirstName, Is.EqualTo(employeeRequest.FirstName));
        Assert.That(createdEmployee.Data.LastName, Is.EqualTo(employeeRequest.LastName));
    }
}