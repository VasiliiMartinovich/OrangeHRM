using System.Text.Json;
using OrangeHRM.Framework.Core.Api.Clients;
using OrangeHRM.Framework.Core.Api.Models.Requests;
using OrangeHRM.Framework.Core.Api.Models.Responses;

namespace OrangeHRM.Tests.Helpers;

public class CandidateVacancyTestDataFactory
{
    private readonly EmployeesApiClient _employeesClient;
    private readonly VacanciesApiClient _vacanciesClient;
    private readonly CandidatesApiClient _candidatesClient;
    private readonly JobTitlesApiClient _jobTitlesClient;
    
    public int? EmployeeId { get; private set; }
    public int? VacancyId { get; private set; }
    public int? CandidateId { get; private set; }
    public int? JobTitleId { get; private set; }

    public string? VacancyName { get; private set; }

    public CreateCandidateRequest? CandidateRequest { get; private set; }

    public CandidateVacancyTestDataFactory(
        EmployeesApiClient employeesClient,
        VacanciesApiClient vacanciesClient,
        CandidatesApiClient candidatesClient,
        JobTitlesApiClient jobTitlesClient)
    {
        _employeesClient = employeesClient;
        _vacanciesClient = vacanciesClient;
        _candidatesClient = candidatesClient;
        _jobTitlesClient = jobTitlesClient;
    }

    public int CreateEmployee()
    {
        var request = new CreateEmployeeRequest
        {
            FirstName = $"Manager{Guid.NewGuid().ToString("N")[..6]}",
            LastName = "ApiTest"
        };

        var response = _employeesClient.CreateEmployee(request);

        Assert.That(response.IsSuccessful, Is.True,
            $"Employee creation failed. HTTP: {(int)response.StatusCode}. " +
            $"Body: {response.Content}");

        var result = JsonSerializer.Deserialize<CreateEmployeeResponse>(response.Content!);

        Assert.That(result?.Data, Is.Not.Null);

        EmployeeId = result!.Data!.EmpNumber;

        Console.WriteLine($"Created employee ID: {EmployeeId}");

        return EmployeeId.Value;
    }

    public int CreateVacancy(int employeeId, int jobTitleId)
    {
        VacancyName = $"ApiTest Vacancy {Guid.NewGuid().ToString("N")[..8]}";

        var request = new CreateVacancyRequest
        {
            Name = VacancyName,
            Status = true,
            JobTitleId = jobTitleId,
            IsPublished = true,
            Description = "Vacancy created by API automation test",
            NumOfPositions = 1,
            EmployeeId = employeeId
        };

        var response = _vacanciesClient.CreateVacancy(request);

        Assert.That(response.IsSuccessful, Is.True,
            $"Vacancy creation failed. HTTP: {(int)response.StatusCode}. " +
            $"Body: {response.Content}");

        var result = JsonSerializer.Deserialize<CreateVacancyResponse>(response.Content!);

        Assert.That(result?.Data, Is.Not.Null);

        VacancyId = result!.Data!.Id;

        Console.WriteLine($"Created vacancy ID: {VacancyId}, name: {VacancyName}");

        return VacancyId.Value;
    }

    public CreateCandidateResponse CreateCandidate(int vacancyId)
    {
        CandidateRequest = new CreateCandidateRequest
        {
            FirstName = $"Candidate{Guid.NewGuid().ToString("N")[..6]}",
            MiddleName = "Api",
            LastName = "Test",
            Email = $"candidate.{Guid.NewGuid():N}@example.com",
            ContactNumber = "1234567890",
            VacancyId = vacancyId,
            Keywords = "API automation",
            Comment = "Candidate created by API automation test",
            DateOfApplication = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            ConsentToKeepData = true
        };

        var response = _candidatesClient.CreateCandidate(CandidateRequest);

        Console.WriteLine($"Candidate creation HTTP status: {(int)response.StatusCode}");

        Assert.That(response.IsSuccessful, Is.True,
            $"Candidate creation failed. HTTP: {(int)response.StatusCode}. " +
            $"Body: {response.Content}");

        var result = JsonSerializer.Deserialize<CreateCandidateResponse>(response.Content!);

        Assert.That(result?.Data, Is.Not.Null);

        CandidateId = result!.Data!.Id;

        Console.WriteLine($"Created candidate ID: {CandidateId}");

        return result;
    }
    
    public int CreateJobTitle()
    {
        var request = new CreateJobTitleRequest
        {
            Title = $"ApiTest Job {Guid.NewGuid().ToString("N")[..8]}",
            Description = "Job title created by API automation test",
            Note = "Created during automated testing"
        };

        var response = _jobTitlesClient.CreateJobTitle(request);

        Assert.That(response.IsSuccessful, Is.True,
            $"Job title creation failed. HTTP: {(int)response.StatusCode}. " +
            $"Body: {response.Content}");

        var result =
            JsonSerializer.Deserialize<CreateJobTitleResponse>(response.Content!);

        Assert.That(result?.Data, Is.Not.Null);

        JobTitleId = result!.Data!.Id;

        Console.WriteLine($"Created job title ID: {JobTitleId}");

        return JobTitleId.Value;
    }
}
