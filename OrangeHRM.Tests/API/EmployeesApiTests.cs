using System.Text.Json;
using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using OrangeHRM.Framework.Core.Api.Clients;
using OrangeHRM.Framework.Core.Api.Models.Requests;
using OrangeHRM.Framework.Core.Api.Models.Responses;
using OrangeHRM.Tests.Helpers;

namespace OrangeHRM.Tests.API;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class EmployeesApiTests : BaseApiTest
{
    [Test]
    [AllureName("GET Employees - successful response with access token")]
    [AllureTag("API", "Employees", "Smoke")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Verifies that an authenticated request to retrieve employees completes without exception and returns HTTP 200.")]
    public void GetEmployees_WithAccessToken_ShouldReturnSuccess()
    {
        Log.Info("Starting test: get employees with access token.");

        using var client = new EmployeesApiClient(AccessToken);

        var response = AllureApi.Step(
            "Send GET request to retrieve employees",
            () =>
            {
                Log.Info("Sending request to get employees.");
                return client.GetEmployees();
            });

        Log.Info($"GET employees response received. HTTP status: {(int)response.StatusCode}.");

        AllureApi.Step("Verify employees API response", () =>
        {
            Assert.That(response.ErrorException, Is.Null,
                "An exception occurred while sending the request.");

            Assert.That((int)response.StatusCode, Is.EqualTo(200),
                "Expected HTTP 200 from the employees API.");
        });

        Log.Info("Test passed: employees API returned HTTP 200.");
    }

    [Test]
    [AllureName("Create Employee - employee ID and information are returned")]
    [AllureTag("API", "Employees", "Smoke")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Verifies that a new employee can be created through the API, the response contains a positive employee ID, and the returned first and last names match the request. The created employee is deleted during cleanup.")]
    public void CreateEmployee_ShouldReturnEmployeeId()
    {
        Log.Info("Starting test: create employee.");

        using var client = new EmployeesApiClient(AccessToken);

        int? employeeNumber = null;

        try
        {
            var employeeRequest = new CreateEmployeeRequest
            {
                FirstName = $"Test{Guid.NewGuid().ToString("N")[..6]}",
                LastName = "ApiTest"
            };

            var response = AllureApi.Step(
                "Create employee with generated test data",
                () =>
                {
                    Log.Info("Creating employee with generated test data.");
                    return client.CreateEmployee(employeeRequest);
                });

            Log.Info($"Create employee response received. HTTP status: {(int)response.StatusCode}.");

            AllureApi.Step("Verify employee creation response", () =>
            {
                Assert.That(response.IsSuccessful, Is.True,
                    $"Employee creation failed. HTTP status: {(int)response.StatusCode}.");

                Assert.That(response.Content, Is.Not.Null.And.Not.Empty,
                    "Employee creation response body was empty.");
            });

            var createdEmployee = AllureApi.Step(
                "Deserialize created employee response",
                () =>
                {
                    Log.Info("Employee creation request succeeded. Deserializing response.");

                    return JsonSerializer.Deserialize<CreateEmployeeResponse>(response.Content!);
                });

            AllureApi.Step("Verify employee data is present", () =>
            {
                Assert.That(createdEmployee, Is.Not.Null,
                    "Could not deserialize employee creation response.");

                Assert.That(createdEmployee!.Data, Is.Not.Null,
                    "Employee data was missing from the response.");
            });

            employeeNumber = createdEmployee!.Data!.EmpNumber;

            AllureApi.Step("Verify created employee information", () =>
            {
                Assert.That(employeeNumber, Is.GreaterThan(0),
                    "Expected a positive employee ID.");

                Assert.That(createdEmployee.Data.FirstName, Is.EqualTo(employeeRequest.FirstName),
                    "The returned first name did not match the request.");

                Assert.That(createdEmployee.Data.LastName, Is.EqualTo(employeeRequest.LastName),
                    "The returned last name did not match the request.");
            });

            Log.Info("Test passed: employee was created and returned an ID greater than zero.");
        }
        finally
        {
            AllureApi.Step("Clean up employee test data", () =>
            {
                if (employeeNumber.HasValue)
                {
                    Log.Info($"Deleting test employee. Employee number: {employeeNumber.Value}.");

                    TestDataCleanup.TryDelete(
                        "employee",
                        employeeNumber,
                        client.DeleteEmployee);
                }
                else
                {
                    Log.Warn("Employee cleanup skipped because no employee number was obtained.");
                }

                Log.Info("Employee test data cleanup attempt completed.");
            });
        }
    }
}