using System.Text.Json;
using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using OrangeHRM.Framework.Core.Api.Clients;
using OrangeHRM.Framework.Core.Api.Models.Requests;
using OrangeHRM.Framework.Core.Api.Models.Responses;
using OrangeHRM.Tests.Helpers;

namespace OrangeHRM.Tests.API;

[TestFixture]
public class VacanciesApiTests : BaseApiTest
{
    [Test]
    [AllureName("POST Vacancy - create vacancy and verify information")]
    [AllureTag("API", "Vacancies")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Creates an employee and an active job title, creates a vacancy, verifies the vacancy information, and attempts to clean up all created test data.")]
    public void CreateVacancy_ShouldReturnVacancyInformation()
    {
        Log.Info("Starting test: CreateVacancy_ShouldReturnVacancyInformation.");

        using var employeesClient = new EmployeesApiClient(AccessToken);
        using var vacanciesClient = new VacanciesApiClient(AccessToken);
        using var jobTitlesClient = new JobTitlesApiClient(AccessToken);

        int? employeeNumber = null;
        int? jobTitleId = null;
        int? vacancyId = null;

        try
        {
            var employeeRequest = new CreateEmployeeRequest
            {
                FirstName = $"Manager{Guid.NewGuid().ToString("N")[..6]}",
                LastName = "ApiTest"
            };

            var employeeResponse = AllureApi.Step(
                "Create employee for vacancy hiring manager",
                () =>
                {
                    Log.Info("Creating employee to use as the vacancy hiring manager.");
                    return employeesClient.CreateEmployee(employeeRequest);
                });

            Log.Info(
                $"Create employee returned HTTP {(int)employeeResponse.StatusCode}. " +
                $"Response body: {employeeResponse.Content}");

            AllureApi.Step("Verify employee creation response", () =>
            {
                Assert.That(employeeResponse.IsSuccessful, Is.True,
                    $"Employee creation failed. HTTP status: {(int)employeeResponse.StatusCode}. " +
                    $"Body: {employeeResponse.Content}");

                Assert.That(employeeResponse.Content, Is.Not.Null.And.Not.Empty,
                    "Employee creation response body was empty.");
            });

            var createdEmployee = AllureApi.Step(
                "Deserialize created employee response",
                () => JsonSerializer.Deserialize<CreateEmployeeResponse>(
                    employeeResponse.Content!));

            Assert.That(createdEmployee?.Data, Is.Not.Null,
                "The API did not return the created employee.");

            employeeNumber = createdEmployee!.Data!.EmpNumber;

            Assert.That(employeeNumber, Is.GreaterThan(0),
                "The created employee number should be greater than zero.");

            Log.Info($"Employee created successfully. Employee number: {employeeNumber.Value}.");

            var jobTitleRequest = new CreateJobTitleRequest
            {
                Title = $"ApiTest Job {Guid.NewGuid().ToString("N")[..8]}",
                Description = "Job title created by API automation test",
                Note = "Created during automated testing"
            };

            var jobTitleResponse = AllureApi.Step(
                "Create job title for vacancy",
                () =>
                {
                    Log.Info($"Creating job title: {jobTitleRequest.Title}.");
                    return jobTitlesClient.CreateJobTitle(jobTitleRequest);
                });

            Log.Info(
                $"Create job title returned HTTP {(int)jobTitleResponse.StatusCode}. " +
                $"Response body: {jobTitleResponse.Content}");

            AllureApi.Step("Verify job title creation response", () =>
            {
                Assert.That(jobTitleResponse.IsSuccessful, Is.True,
                    $"Job title creation failed. HTTP status: {(int)jobTitleResponse.StatusCode}. " +
                    $"Body: {jobTitleResponse.Content}");

                Assert.That(jobTitleResponse.Content, Is.Not.Null.And.Not.Empty,
                    "Job title creation response body was empty.");
            });

            var createdJobTitle = AllureApi.Step(
                "Deserialize created job title response",
                () => JsonSerializer.Deserialize<CreateJobTitleResponse>(
                    jobTitleResponse.Content!));

            Assert.That(createdJobTitle?.Data, Is.Not.Null,
                "The API did not return the created job title.");

            jobTitleId = createdJobTitle!.Data!.Id;

            Assert.That(jobTitleId, Is.GreaterThan(0),
                "The created job title ID should be greater than zero.");

            Log.Info(
                $"Job title created successfully. ID: {jobTitleId.Value}, " +
                $"title: {jobTitleRequest.Title}.");

            var vacancyRequest = new CreateVacancyRequest
            {
                Name = $"ApiTest Vacancy {Guid.NewGuid().ToString("N")[..8]}",
                Status = true,
                JobTitleId = jobTitleId.Value,
                IsPublished = true,
                Description = "Vacancy created by API automation test",
                NumOfPositions = 1,
                EmployeeId = employeeNumber.Value
            };

            var vacancyResponse = AllureApi.Step(
                "Create vacancy using the employee as hiring manager",
                () =>
                {
                    Log.Info("Sending request to create vacancy.");
                    return vacanciesClient.CreateVacancy(vacancyRequest);
                });

            Log.Info(
                $"Create vacancy returned HTTP {(int)vacancyResponse.StatusCode}. " +
                $"Response body: {vacancyResponse.Content}");

            AllureApi.Step("Verify vacancy creation response", () =>
            {
                Assert.That(vacancyResponse.IsSuccessful, Is.True,
                    $"Vacancy creation failed. HTTP status: {(int)vacancyResponse.StatusCode}. " +
                    $"Body: {vacancyResponse.Content}");

                Assert.That(vacancyResponse.Content, Is.Not.Null.And.Not.Empty,
                    "Vacancy creation response body was empty.");
            });

            var createdVacancy = AllureApi.Step(
                "Deserialize created vacancy response",
                () => JsonSerializer.Deserialize<CreateVacancyResponse>(
                    vacancyResponse.Content!));

            Assert.That(createdVacancy?.Data, Is.Not.Null,
                "The API did not return the created vacancy.");

            vacancyId = createdVacancy!.Data!.Id;

            AllureApi.Step("Verify created vacancy information", () =>
            {
                Assert.That(vacancyId, Is.GreaterThan(0),
                    "The created vacancy ID should be greater than zero.");

                Assert.That(
                    createdVacancy.Data.Name,
                    Is.EqualTo(vacancyRequest.Name));

                Assert.That(
                    createdVacancy.Data.Status,
                    Is.EqualTo(vacancyRequest.Status));

                Assert.That(
                    createdVacancy.Data.IsPublished,
                    Is.EqualTo(vacancyRequest.IsPublished));

                Assert.That(
                    createdVacancy.Data.JobTitle?.Id,
                    Is.EqualTo(jobTitleId.Value));

                Assert.That(
                    createdVacancy.Data.HiringManager?.Id,
                    Is.EqualTo(employeeNumber.Value));
            });

            Log.Info(
                $"Test passed: vacancy information was verified. Vacancy ID: {vacancyId.Value}.");
        }
        finally
        {
            AllureApi.Step("Clean up vacancy test data", () =>
            {
                Log.Info("Starting cleanup of vacancy test data.");

                if (vacancyId.HasValue)
                {
                    TestDataCleanup.TryDelete(
                        "vacancy",
                        vacancyId,
                        vacanciesClient.DeleteVacancy);
                }
                else
                {
                    Log.Warn("Vacancy cleanup skipped because no vacancy ID was obtained.");
                }

                if (employeeNumber.HasValue)
                {
                    TestDataCleanup.TryDelete(
                        "employee",
                        employeeNumber,
                        employeesClient.DeleteEmployee);
                }
                else
                {
                    Log.Warn("Employee cleanup skipped because no employee number was obtained.");
                }

                if (jobTitleId.HasValue)
                {
                    TestDataCleanup.TryDelete(
                        "job title",
                        jobTitleId,
                        jobTitlesClient.DeleteJobTitle);
                }
                else
                {
                    Log.Warn("Job title cleanup skipped because no job title ID was obtained.");
                }

                Log.Info("Vacancy test data cleanup attempts completed.");
            });
        }
    }
}