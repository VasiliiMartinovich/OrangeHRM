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
        "Creates an employee to act as the vacancy hiring manager, creates a vacancy, and verifies that the response contains the expected vacancy name, status, publication state, job title, and hiring manager. Attempts to clean up both created records.")]
    public void CreateVacancy_ShouldReturnVacancyInformation()
    {
        Log.Info("Starting test: CreateVacancy_ShouldReturnVacancyInformation.");

        using var employeesClient = new EmployeesApiClient(AccessToken);
        using var vacanciesClient = new VacanciesApiClient(AccessToken);

        int? employeeNumber = null;
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

            Log.Info($"Create employee returned HTTP {(int)employeeResponse.StatusCode}.");

            AllureApi.Step("Verify employee creation response", () =>
            {
                Assert.That(employeeResponse.IsSuccessful, Is.True,
                    $"Employee creation failed. HTTP status: {(int)employeeResponse.StatusCode}.");

                Assert.That(employeeResponse.Content, Is.Not.Null.And.Not.Empty,
                    "Employee creation response body was empty.");
            });

            var createdEmployee = AllureApi.Step(
                "Deserialize created employee response",
                () =>
                {
                    Log.Info("Deserializing created employee response.");

                    return JsonSerializer.Deserialize<CreateEmployeeResponse>(employeeResponse.Content!);
                });

            AllureApi.Step("Verify created employee information", () =>
            {
                Assert.That(createdEmployee?.Data, Is.Not.Null,
                    "The API did not return the created employee.");
            });

            employeeNumber = createdEmployee!.Data!.EmpNumber;

            AllureApi.Step("Verify employee number is valid", () =>
            {
                Assert.That(employeeNumber, Is.GreaterThan(0),
                    "The created employee number should be greater than zero.");
            });

            Log.Info($"Employee created successfully. Employee number: {employeeNumber.Value}.");

            var vacancyRequest = new CreateVacancyRequest
            {
                Name = $"ApiTest Vacancy {Guid.NewGuid().ToString("N")[..8]}",
                Status = true,
                JobTitleId = 22,
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

            Log.Info($"Create vacancy returned HTTP {(int)vacancyResponse.StatusCode}.");

            AllureApi.Step("Verify vacancy creation response", () =>
            {
                Assert.That(vacancyResponse.IsSuccessful, Is.True,
                    $"Vacancy creation failed. HTTP status: {(int)vacancyResponse.StatusCode}.");

                Assert.That(vacancyResponse.Content, Is.Not.Null.And.Not.Empty,
                    "Vacancy creation response body was empty.");
            });

            var createdVacancy = AllureApi.Step(
                "Deserialize created vacancy response",
                () =>
                {
                    Log.Info("Deserializing created vacancy response.");

                    return JsonSerializer.Deserialize<CreateVacancyResponse>(vacancyResponse.Content!);
                });

            AllureApi.Step("Verify created vacancy response contains data", () =>
            {
                Assert.That(createdVacancy?.Data, Is.Not.Null,
                    "The API did not return the created vacancy.");
            });

            vacancyId = createdVacancy!.Data!.Id;

            AllureApi.Step("Verify created vacancy information", () =>
            {
                Assert.That(vacancyId, Is.GreaterThan(0),
                    "The created vacancy ID should be greater than zero.");

                Assert.That(createdVacancy.Data.Name, Is.EqualTo(vacancyRequest.Name));

                Assert.That(createdVacancy.Data.Status, Is.EqualTo(vacancyRequest.Status));

                Assert.That(createdVacancy.Data.IsPublished, Is.EqualTo(vacancyRequest.IsPublished));

                Assert.That(createdVacancy.Data.JobTitle?.Id, Is.EqualTo(vacancyRequest.JobTitleId));

                Assert.That(createdVacancy.Data.HiringManager?.Id, Is.EqualTo(employeeNumber.Value));
            });

            Log.Info($"Test passed: vacancy information was verified. Vacancy ID: {vacancyId.Value}.");
        }
        finally
        {
            AllureApi.Step("Clean up vacancy test data", () =>
            {
                Log.Info("Starting cleanup of vacancy test data.");

                if (vacancyId.HasValue)
                {
                    Log.Info($"Deleting test vacancy. Vacancy ID: {vacancyId.Value}.");

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
                    Log.Info($"Deleting test employee. Employee number: {employeeNumber.Value}.");

                    TestDataCleanup.TryDelete(
                        "employee",
                        employeeNumber,
                        employeesClient.DeleteEmployee);
                }
                else
                {
                    Log.Warn("Employee cleanup skipped because no employee number was obtained.");
                }

                Log.Info("Vacancy test data cleanup attempts completed.");
            });
        }
    }
}