using OrangeHRM.Tests.Configuration;
using System.Text.Json;
using OrangeHRM.Framework.Core.Api.Clients;
using OrangeHRM.Framework.Core.Api.Models.Requests;
using OrangeHRM.Framework.Core.Api.Models.Responses;

namespace OrangeHRM.Tests.Infrastructure;

public class TestDataSetup
{
    private string AccessToken => TestSettings.AccessToken;

    public string CreateEmployeeForHiringManager()
    {
        var employee = CreateEmployee();

        return $"{employee.FirstName} {employee.MiddleName} {employee.LastName}";
    }

    public string CreateVacancyForHiringManager()
    {
        var employee = CreateEmployee();

        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var jobTitleName = $"TestJob{uniqueId}";
        var vacancyName = $"QA Automation Vacancy {uniqueId}";

        using var jobTitlesApiClient = new JobTitlesApiClient(AccessToken);
        //Console.WriteLine($"Creating Job Title: '{jobTitleName}'");
        var jobTitleResponse = jobTitlesApiClient.CreateJobTitle(
            new CreateJobTitleRequest
            {
                Title = jobTitleName,
                Description = "Job Title for automated tests",
                Note = "Test data"
            });

        if (!jobTitleResponse.IsSuccessful)
        {
            throw new InvalidOperationException(
                $"Failed to create Job Title. " +
                $"Status: {jobTitleResponse.StatusCode}. " +
                $"Response: {jobTitleResponse.Content}");
        }

        var jobTitle = JsonSerializer.Deserialize<CreateJobTitleResponse>(
            jobTitleResponse.Content ?? string.Empty);

        if (jobTitle?.Data == null || jobTitle.Data.Id <= 0)
        {
            throw new InvalidOperationException("Job Title was created, but its ID could not be read.");
        }

        using var vacanciesApiClient = new VacanciesApiClient(AccessToken);

        var vacancyResponse = vacanciesApiClient.CreateVacancy(new CreateVacancyRequest
        {
            Name = vacancyName,
            Status = true,
            JobTitleId = jobTitle.Data.Id,
            IsPublished = false,
            Description = "Vacancy for automated tests",
            NumOfPositions = 1,
            EmployeeId = employee.EmpNumber
        });

        if (!vacancyResponse.IsSuccessful)
        {
            throw new InvalidOperationException(
                $"Failed to create Vacancy. " +
                $"Status: {vacancyResponse.StatusCode}. " +
                $"Response: {vacancyResponse.Content}");
        }

        var vacancy = JsonSerializer.Deserialize<CreateVacancyResponse>(
            vacancyResponse.Content ?? string.Empty);

        if (vacancy?.Data == null || vacancy.Data.Id <= 0)
        {
            throw new InvalidOperationException(
                "Vacancy was created, but its ID could not be read.");
        }

        return vacancy.Data.Name;
    }

    private EmployeeDto CreateEmployee()
    {
        var uniqueId = DateTime.Now.ToString("yyyyMMddHHmmssfff");

        var firstName = $"Manager{uniqueId}";
        var middleName = "Test";
        var lastName = $"Employee{uniqueId}";
        var employeeId = Guid.NewGuid().ToString("N")[..8];

        using var employeesApiClient = new EmployeesApiClient(AccessToken);

        var response = employeesApiClient.CreateEmployee(new CreateEmployeeRequest
        {
            FirstName = firstName,
            MiddleName = middleName,
            LastName = lastName,
            EmployeeId = employeeId
        });

        if (!response.IsSuccessful)
        {
            throw new InvalidOperationException(
                $"Failed to create Employee. " +
                $"Status: {response.StatusCode}. " +
                $"Response: {response.Content}");
        }

        var employee = JsonSerializer.Deserialize<CreateEmployeeResponse>(
            response.Content ?? string.Empty);

        if (employee?.Data == null || employee.Data.EmpNumber <= 0)
        {
            throw new InvalidOperationException(
                "Employee was created, but its ID could not be read.");
        }

        return employee.Data;
    }
}