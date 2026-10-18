using System.Text.Json;
using OrangeHRM.Framework.Core.Api.Clients;
using OrangeHRM.Framework.Core.Api.Models.Requests;
using OrangeHRM.Framework.Core.Api.Models.Responses;

namespace OrangeHRM.Tests.API;

[TestFixture]
public class VacanciesApiTests : BaseApiTest
{
    [Test]
    public void CreateVacancy_ShouldReturnVacancyInformation()
    {
        using var employeesClient = new EmployeesApiClient(AccessToken);
        using var vacanciesClient = new VacanciesApiClient(AccessToken);
        
        var employeeRequest = new CreateEmployeeRequest
        {
            FirstName = $"Manager{Guid.NewGuid().ToString("N")[..6]}",
            LastName = "ApiTest"
        };

        var employeeResponse = employeesClient.CreateEmployee(employeeRequest);

        Assert.That(employeeResponse.IsSuccessful, Is.True,
            $"Employee creation failed. " +
            $"Status: {(int)employeeResponse.StatusCode}. " +
            $"Body: {employeeResponse.Content}");

        var createdEmployee = JsonSerializer.Deserialize<CreateEmployeeResponse>(employeeResponse.Content!);

        Assert.That(createdEmployee?.Data, Is.Not.Null);

        var employeeNumber = createdEmployee!.Data!.EmpNumber;

        var vacancyRequest = new CreateVacancyRequest
        {
            Name = $"ApiTest Vacancy {Guid.NewGuid().ToString("N")[..8]}",
            Status = true,
            JobTitleId = 22,
            IsPublished = true,
            Description = "Vacancy created by API automation test",
            NumOfPositions = 1,
            EmployeeId = employeeNumber
        };

        var vacancyResponse = vacanciesClient.CreateVacancy(vacancyRequest);

        Console.WriteLine($"Status: {(int)vacancyResponse.StatusCode}");
        Console.WriteLine($"Body: {vacancyResponse.Content}");

        Assert.That(vacancyResponse.IsSuccessful, Is.True,
            $"Vacancy creation failed. " +
            $"Status: {(int)vacancyResponse.StatusCode}. " +
            $"Body: {vacancyResponse.Content}");

        var createdVacancy = JsonSerializer.Deserialize<CreateVacancyResponse>(vacancyResponse.Content!);

        Assert.That(createdVacancy?.Data, Is.Not.Null);
        Assert.That(createdVacancy!.Data!.Id, Is.GreaterThan(0));
        Assert.That(createdVacancy.Data.Name, Is.EqualTo(vacancyRequest.Name));
        Assert.That(createdVacancy.Data.Status, Is.EqualTo(vacancyRequest.Status));
        Assert.That(createdVacancy.Data.IsPublished, Is.EqualTo(vacancyRequest.IsPublished));
        Assert.That(createdVacancy.Data.JobTitle?.Id, Is.EqualTo(vacancyRequest.JobTitleId));
        Assert.That(createdVacancy.Data.HiringManager?.Id, Is.EqualTo(employeeNumber));
    }
}