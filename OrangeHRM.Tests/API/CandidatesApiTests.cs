using System.Text.Json;
using OrangeHRM.Framework.Core.Api.Clients;
using OrangeHRM.Framework.Core.Api.Models.Responses;
using OrangeHRM.Framework.Core.Api.Models.Requests;
using OrangeHRM.Tests.Helpers;

namespace OrangeHRM.Tests.API;

[TestFixture]
public class CandidatesApiTests : BaseApiTest
{
    [Test]
    public void GetCandidates_ShouldReturnValidResponseStructure()
    {
        using var client = new CandidatesApiClient(AccessToken);

        var response = client.GetCandidates();

        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

        var result = JsonSerializer.Deserialize<CandidatesResponse>(
            response.Content!);

        Assert.That(result, Is.Not.Null, "Response could not be deserialized.");
        Assert.That(result!.Data, Is.Not.Null);
        Assert.That(result.Meta, Is.Not.Null);
        Assert.That(result.Meta!.Total, Is.GreaterThanOrEqualTo(0));
        Assert.That(result.Rels, Is.Not.Null);
    }

    [Test]
    public void GetCandidates_ShouldReturnCandidatesWithRequiredFields()
    {
        using var client = new CandidatesApiClient(AccessToken);

        var response = client.GetCandidates();

        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

        var result = JsonSerializer.Deserialize<CandidatesResponse>(response.Content!);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Data, Is.Not.Empty);

        foreach (var candidate in result.Data)
        {
            Assert.That(candidate.Id, Is.GreaterThan(0));
        }
    }

    [Test]
    public void GetCandidates_WithoutToken_ShouldRejectRequest()
    {
        using var client = new CandidatesApiClientWithoutAuthorization();

        var response = client.GetCandidates();

        Console.WriteLine($"Status code without token: {(int)response.StatusCode}");

        Assert.That((int)response.StatusCode, Is.AnyOf(401, 403),
            "Expected the API to reject a request without authorization.");
    }

    private static void AssertStringProperty(
        JsonElement candidate,
        string propertyName,
        bool allowNull = false)
    {
        Assert.That(candidate.TryGetProperty(propertyName, out var property), Is.True,
            $"Candidate should contain {propertyName}.");

        var isString = property.ValueKind == JsonValueKind.String;
        var isAllowedNull = allowNull && property.ValueKind == JsonValueKind.Null;

        Assert.That(isString || isAllowedNull, Is.True,
            $"Property {propertyName} should be a string" +
            (allowNull ? " or null." : "."));
    }

    [Test]
    public void GetCandidates_WithInvalidToken_ShouldNotReturnJson()
    {
        using var client = new CandidatesApiClient("invalid-token");

        var response = client.GetCandidates();

        Console.WriteLine($"Status code: {(int)response.StatusCode}");
        Console.WriteLine($"Content type: {response.ContentType}");

        Assert.That(response.ContentType, Does.Not.Contain("application/json"),
            "Unexpected JSON response for an invalid token.");
    }

    [Test]
    public void GetCandidates_WithCandidateId_ShouldReturnMatchingCandidate()
    {
        using var client = new CandidatesApiClient(AccessToken);

        var uniqueId = Guid.NewGuid().ToString("N")[..8];

        var candidateRequest = new CreateCandidateRequest
        {
            FirstName = $"Api{uniqueId}",
            MiddleName = "",
            LastName = "TestCandidate",
            Email = $"candidate{uniqueId}@example.com"
        };

        int? candidateId = null;

        try
        {
            // Arrange: create a candidate.
            var createResponse = client.CreateCandidate(candidateRequest);

            Assert.That(createResponse.IsSuccessful, Is.True,
                $"Candidate creation failed. " +
                $"Status: {(int)createResponse.StatusCode}. " +
                $"Body: {createResponse.Content}");

            var createdCandidate =
                JsonSerializer.Deserialize<CreateCandidateResponse>(
                    createResponse.Content!);

            Assert.That(createdCandidate?.Data, Is.Not.Null,
                "The API did not return the created candidate.");

            candidateId = createdCandidate!.Data!.Id;

            Assert.That(candidateId, Is.GreaterThan(0),
                "The created candidate ID should be greater than zero.");

            // Act: retrieve the candidate by the ID returned by the API.
            var response = client.GetCandidates(candidateId.Value);

            Assert.That(response.IsSuccessful, Is.True,
                $"Get candidate failed. " +
                $"Status: {(int)response.StatusCode}. " +
                $"Body: {response.Content}");

            var result = JsonSerializer.Deserialize<CandidatesResponse>(
                response.Content!);

            // Assert: verify the returned candidate.
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Data, Is.Not.Empty);

            Assert.That(result.Data, Has.All.Matches<CandidateDto>(candidate => candidate.Id == candidateId.Value));

            Assert.That(result.Data.Any(candidate =>
                    candidate.FirstName == candidateRequest.FirstName &&
                    candidate.LastName == candidateRequest.LastName &&
                    candidate.Email == candidateRequest.Email),
                Is.True,
                "The response did not contain the candidate that was created.");
        }
        finally
        {
            // Cleanup: delete the candidate created by this test.
            TestDataCleanup.TryDelete(
                "candidate",
                candidateId,
                client.DeleteCandidate);
        }
    }

    [Test]
    public void GetCandidates_WithNonExistingCandidateId_ShouldReturnEmptyList()
    {
        using var client = new CandidatesApiClient(AccessToken);

        var response = client.GetCandidates(999999991);

        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

        var result = JsonSerializer.Deserialize<CandidatesResponse>(response.Content!);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Data, Is.Empty,
            "Expected no candidates for a non-existing ID.");
    }

    [Test]
    public void GetCandidates_WithNonExistingCandidateName_ShouldReturnEmptyList()
    {
        using var client = new CandidatesApiClient(AccessToken);

        var response = client.GetCandidates(candidateName: "NoSuchCandidate_mIk_9018");

        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

        var result = JsonSerializer.Deserialize<CandidatesResponse>(response.Content!);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Data, Is.Empty,
            "Expected no candidates for a non-existing name.");
    }

    [Test]
    public void CreateCandidate_ShouldReturnCreatedCandidate()
    {
        using var client = new CandidatesApiClient(AccessToken);
        var candidate = new CreateCandidateRequest
        {
            FirstName = "ApiTest",
            LastName = "Candidate",
            Email = $"apitest.{Guid.NewGuid():N}@example.com"
        };
        var response = client.CreateCandidate(candidate);

        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

        var result = JsonSerializer.Deserialize<CreateCandidateResponse>(response.Content!);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Data, Is.Not.Null);
        Assert.That(result.Data!.Id, Is.GreaterThan(0));
        Assert.That(result.Data.FirstName, Is.EqualTo(candidate.FirstName));
        Assert.That(result.Data.LastName, Is.EqualTo(candidate.LastName));
        Assert.That(result.Data.Email, Is.EqualTo(candidate.Email));
    }

    [Test]
    public void CreateCandidate_WithoutFirstName_ShouldReturnValidationError()
    {
        using var client = new CandidatesApiClient(AccessToken);

        var candidate = new CreateCandidateRequest
        {
            LastName = "Candidate",
            Email = $"apitest.{Guid.NewGuid():N}@example.com"
        };

        var response = client.CreateCandidate(candidate);

        Console.WriteLine($"Status code: {(int)response.StatusCode}");
        Console.WriteLine($"Response body: {response.Content}");

        Assert.That((int)response.StatusCode, Is.EqualTo(422));

        var error = JsonSerializer.Deserialize<ValidationErrorResponse>(response.Content!);

        Assert.That(error, Is.Not.Null);
        Assert.That(error!.Error.Status, Is.EqualTo("422"));
        Assert.That(error.Error.Message, Is.EqualTo("Invalid Parameter"));
        Assert.That(error.Error.Data.InvalidParamKeys, Does.Contain("firstName"));
    }

    [Test]
    public void CreateCandidate_WithoutLastName_ShouldReturnValidationError()
    {
        using var client = new CandidatesApiClient(AccessToken);

        var candidate = new CreateCandidateRequest
        {
            FirstName = "ApiTest",
            Email = $"apitest.{Guid.NewGuid():N}@example.com"
        };

        var response = client.CreateCandidate(candidate);

        Console.WriteLine($"Status code: {(int)response.StatusCode}");
        Console.WriteLine($"Response body: {response.Content}");

        Assert.That((int)response.StatusCode, Is.EqualTo(422));

        var error = JsonSerializer.Deserialize<ValidationErrorResponse>(response.Content!);

        Assert.That(error, Is.Not.Null);
        Assert.That(error!.Error.Status, Is.EqualTo("422"));
        Assert.That(error.Error.Message, Is.EqualTo("Invalid Parameter"));
        Assert.That(error.Error.Data.InvalidParamKeys, Does.Contain("lastName"));
    }

    [Test]
    public void CreateCandidate_WithoutEmailField_ShouldReturnValidationError()
    {
        using var client = new CandidatesApiClient(AccessToken);

        const string requestBody =
            """
            {
                "firstName": "ApiTest",
                "lastName": "Candidate"
            }
            """;

        var response = client.CreateCandidateWithoutEmailField(requestBody);

        Console.WriteLine($"Status code: {(int)response.StatusCode}");
        Console.WriteLine($"Response body: {response.Content}");

        Assert.That((int)response.StatusCode, Is.EqualTo(422));

        var error = JsonSerializer.Deserialize<ValidationErrorResponse>(response.Content!);

        Assert.That(error, Is.Not.Null);
        Assert.That(error!.Error.Status, Is.EqualTo("422"));
        Assert.That(error.Error.Message, Is.EqualTo("Invalid Parameter"));
        Assert.That(error.Error.Data.InvalidParamKeys, Does.Contain("email"));
    }

    [Test]
    public void UpdateCandidate_ShouldUpdateCandidateInformation()
    {
        using var client = new CandidatesApiClient(AccessToken);
        var createRequest = new CreateCandidateRequest
        {
            FirstName = $"Put{Guid.NewGuid().ToString("N")[..8]}",
            MiddleName = $"Put{Guid.NewGuid().ToString("N")[..8]}",
            LastName = "BeforeUpdate",
            Email = $"put.{Guid.NewGuid():N}@example.com"
        };

        var createResponse = client.CreateCandidate(createRequest);
        Assert.That((int)createResponse.StatusCode, Is.EqualTo(200),
            $"Candidate creation failed: {createResponse.Content}");

        var createdCandidate = JsonSerializer.Deserialize<CreateCandidateResponse>(createResponse.Content!);

        Assert.That(createdCandidate, Is.Not.Null);
        Assert.That(createdCandidate!.Data, Is.Not.Null);

        var candidateId = createdCandidate.Data!.Id;
        try
        {
            var updateRequest = new UpdateCandidateRequest
            {
                FirstName = $"Updated{Guid.NewGuid().ToString("N")[..6]}",
                MiddleName = "AfterUpdate",
                LastName = "AfterUpdate",
                Email = $"updated.{Guid.NewGuid():N}@example.com"
            };

            var updateResponse = client.UpdateCandidate(
                candidateId,
                updateRequest);

            Console.WriteLine($"Candidate ID: {candidateId}");
            Console.WriteLine($"PUT status code: {(int)updateResponse.StatusCode}");
            Console.WriteLine($"PUT response body: {updateResponse.Content}");

            Assert.That((int)updateResponse.StatusCode, Is.EqualTo(200),
                $"Candidate update failed: {updateResponse.Content}");

            var getResponse = client.GetCandidates(candidateId: candidateId);

            Assert.That((int)getResponse.StatusCode, Is.EqualTo(200),
                $"GET after update failed: {getResponse.Content}");

            var candidates = JsonSerializer.Deserialize<CandidatesResponse>(getResponse.Content!);

            Assert.That(candidates, Is.Not.Null);
            Assert.That(candidates!.Data, Has.Count.EqualTo(1));

            var updatedCandidate = candidates.Data[0];

            Assert.That(updatedCandidate.FirstName, Is.EqualTo(updateRequest.FirstName));
            Assert.That(updatedCandidate.MiddleName, Is.EqualTo(updateRequest.MiddleName));
            Assert.That(updatedCandidate.LastName, Is.EqualTo(updateRequest.LastName));
            Assert.That(updatedCandidate.Email, Is.EqualTo(updateRequest.Email));
        }
        finally
        {
            var deleteResponse = client.DeleteCandidate(candidateId);

            Console.WriteLine($"Cleanup DELETE status code: {(int)deleteResponse.StatusCode}");

            Assert.That((int)deleteResponse.StatusCode, Is.EqualTo(200),
                $"Failed to delete test candidate: {deleteResponse.Content}");
        }
    }

    [Test]
    public void GetCandidates_WithCandidateName_ShouldReturnMatchingCandidates()
    {
        using var client = new CandidatesApiClient(AccessToken);

        var uniqueName = $"Api{Guid.NewGuid().ToString("N")[..8]}";

        var candidateRequest = new CreateCandidateRequest
        {
            FirstName = uniqueName,
            LastName = "SearchTest",
            Email = $"apitest.{Guid.NewGuid():N}@example.com"
        };

        var createResponse = client.CreateCandidate(candidateRequest);

        Assert.That((int)createResponse.StatusCode, Is.EqualTo(200),
            $"Candidate creation failed: {createResponse.Content}");

        var createdCandidate = JsonSerializer.Deserialize<CreateCandidateResponse>(createResponse.Content!);

        Assert.That(createdCandidate, Is.Not.Null);
        Assert.That(createdCandidate!.Data, Is.Not.Null);

        var candidateId = createdCandidate.Data!.Id;

        try
        {
            var response = client.GetCandidates(candidateName: uniqueName);

            Console.WriteLine($"Search name: {uniqueName}");
            Console.WriteLine($"Status code: {(int)response.StatusCode}");
            Console.WriteLine($"Response body: {response.Content}");

            Assert.That((int)response.StatusCode, Is.EqualTo(200));

            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

            var result = JsonSerializer.Deserialize<CandidatesResponse>(response.Content!);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Data, Is.Not.Empty);
            Assert.That(result.Data, Has.All.Matches<CandidateDto>(candidate => candidate.FirstName == uniqueName),
                "Search returned candidates that do not match the requested name.");
            Assert.That(result.Data, Has.Some.Matches<CandidateDto>(candidate => candidate.Id == candidateId),
                "The newly created candidate was not found.");
        }
        finally
        {
            var deleteResponse = client.DeleteCandidate(candidateId);

            Console.WriteLine($"Delete status code: {(int)deleteResponse.StatusCode}");
            Console.WriteLine($"Delete response body: {deleteResponse.Content}");

            Assert.That((int)deleteResponse.StatusCode, Is.EqualTo(200),
                "Failed to delete the test candidate.");
            Assert.That(deleteResponse.Content, Is.Not.Null.And.Not.Empty,
                "Delete response body is empty.");

            var deletedCandidate = JsonSerializer.Deserialize<DeleteCandidateResponse>(deleteResponse.Content!);

            Assert.That(deletedCandidate, Is.Not.Null);
            Assert.That(deletedCandidate!.Data, Does.Contain(candidateId.ToString()),
                "The API response does not contain the ID of the deleted candidate.");
        }
    }

    [Test]
    public void DeleteCandidate_ShouldDeleteExistingCandidate()
    {
        using var client = new CandidatesApiClient(AccessToken);

        var candidateRequest = new CreateCandidateRequest
        {
            FirstName = $"Del{Guid.NewGuid().ToString("N")[..8]}",
            LastName = "DeleteTest",
            Email = $"delete.{Guid.NewGuid():N}@example.com"
        };

        var createResponse = client.CreateCandidate(candidateRequest);

        Assert.That((int)createResponse.StatusCode, Is.EqualTo(200),
            $"Candidate creation failed: {createResponse.Content}");

        var createdCandidate = JsonSerializer.Deserialize<CreateCandidateResponse>(createResponse.Content!);

        Assert.That(createdCandidate, Is.Not.Null);
        Assert.That(createdCandidate!.Data, Is.Not.Null);

        var candidateId = createdCandidate.Data!.Id;
        var deleteResponse = client.DeleteCandidate(candidateId);

        Console.WriteLine($"Candidate ID: {candidateId}");
        Console.WriteLine($"Delete status code: {(int)deleteResponse.StatusCode}");
        Console.WriteLine($"Delete response body: {deleteResponse.Content}");

        Assert.That((int)deleteResponse.StatusCode, Is.EqualTo(200),
            $"Candidate deletion failed: {deleteResponse.Content}");
        Assert.That(deleteResponse.Content, Is.Not.Null.And.Not.Empty);

        var deletedCandidate = JsonSerializer.Deserialize<DeleteCandidateResponse>(deleteResponse.Content!);

        Assert.That(deletedCandidate, Is.Not.Null);
        Assert.That(deletedCandidate!.Data, Does.Contain(candidateId.ToString()),
            "The delete response does not contain the ID of the deleted candidate.");

        var getResponse = client.GetCandidates(candidateId: candidateId);

        Console.WriteLine($"GET after deletion status code: {(int)getResponse.StatusCode}");
        Console.WriteLine($"GET after deletion response body: {getResponse.Content}");

        Assert.That((int)getResponse.StatusCode, Is.EqualTo(200));

        var candidatesAfterDeletion = JsonSerializer.Deserialize<CandidatesResponse>(getResponse.Content!);

        Assert.That(candidatesAfterDeletion, Is.Not.Null);
        Assert.That(candidatesAfterDeletion!.Data, Is.Empty,
            "Candidate still exists after deletion.");
    }

    [Test]
    public void DeleteCandidate_WithNonExistingId_ShouldReturnNotFound()
    {
        using var client = new CandidatesApiClient(AccessToken);

        var nonExistingCandidateId = 999999991;
        var response = client.DeleteCandidate(nonExistingCandidateId);

        Console.WriteLine($"Candidate ID: {nonExistingCandidateId}");
        Console.WriteLine($"Status code: {(int)response.StatusCode}");
        Console.WriteLine($"Response body: {response.Content}");

        Assert.That((int)response.StatusCode, Is.EqualTo(404));
        Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

        var errorResponse = JsonSerializer.Deserialize<ValidationErrorResponse>(response.Content!);

        Assert.That(errorResponse, Is.Not.Null);
        Assert.That(errorResponse!.Error.Status, Is.EqualTo("404"));
        Assert.That(errorResponse.Error.Message, Is.EqualTo("Records Not Found"));
    }

    private int CreateTestCandidate(CandidatesApiClient client)
    {
        var request = new CreateCandidateRequest
        {
            FirstName = $"History{Guid.NewGuid().ToString("N")[..8]}",
            LastName = "ApiTest",
            Email = $"history.{Guid.NewGuid():N}@example.com"
        };

        var response = client.CreateCandidate(request);

        Assert.That((int)response.StatusCode, Is.EqualTo(200),
            $"Candidate creation failed: {response.Content}");

        var createdCandidate = JsonSerializer.Deserialize<CreateCandidateResponse>(response.Content!);

        Assert.That(createdCandidate, Is.Not.Null);
        Assert.That(createdCandidate!.Data, Is.Not.Null);

        return createdCandidate.Data!.Id;
    }

    [Test]
    public void GetCandidateHistory_ShouldReturnValidResponse()
    {
        using var employeesClient = new EmployeesApiClient(AccessToken);
        using var vacanciesClient = new VacanciesApiClient(AccessToken);
        using var candidatesClient = new CandidatesApiClient(AccessToken);

        var factory = new CandidateVacancyTestDataFactory(
            employeesClient,
            vacanciesClient,
            candidatesClient);

        try
        {
            var employeeId = factory.CreateEmployee();
            var vacancyId = factory.CreateVacancy(employeeId);
            var candidate = factory.CreateCandidate(vacancyId);
            var candidateId = candidate.Data!.Id;
            var response = candidatesClient.GetCandidateHistory(candidateId);


            Assert.That((int)response.StatusCode, Is.EqualTo(200),
                $"Request failed: {response.Content}");
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);

            var history = JsonSerializer.Deserialize<CandidateHistoryResponse>(response.Content!);

            Assert.That(history, Is.Not.Null);
            Assert.That(history!.Data, Is.Not.Null);
            Assert.That(history.Meta, Is.Not.Null);
            Assert.That(history.Meta.Total, Is.GreaterThanOrEqualTo(0));
            Assert.That(history.Data.Count, Is.EqualTo(history.Meta.Total),
                "The total count should match the number of returned history records.");

            foreach (var item in history.Data)
            {
                Assert.That(item.Id, Is.GreaterThan(0));
                Assert.That(item.Action, Is.Not.Null);
                Assert.That(item.Action!.Id, Is.GreaterThan(0));
                Assert.That(item.Action.Label, Is.Not.Empty);
                Assert.That(item.PerformedBy, Is.Not.Null);
                Assert.That(item.PerformedBy!.EmpNumber, Is.GreaterThan(0));
                Assert.That(item.PerformedDate, Is.Not.Empty);
            }
        }
        finally
        {
            TestDataCleanup.TryDelete(
                "candidate",
                factory.CandidateId,
                candidatesClient.DeleteCandidate);

            TestDataCleanup.TryDelete(
                "vacancy",
                factory.VacancyId,
                vacanciesClient.DeleteVacancy);

            TestDataCleanup.TryDelete(
                "employee",
                factory.EmployeeId,
                employeesClient.DeleteEmployee);
        }
    }

    [Test]
    public void CreateCandidateForVacancy_ShouldReturnCandidateInformationAndHistory()
    {
        using var employeesClient = new EmployeesApiClient(AccessToken);
        using var vacanciesClient = new VacanciesApiClient(AccessToken);
        using var candidatesClient = new CandidatesApiClient(AccessToken);

        var factory = new CandidateVacancyTestDataFactory(
            employeesClient,
            vacanciesClient,
            candidatesClient);

        try
        {
            var employeeId = factory.CreateEmployee();
            var vacancyId = factory.CreateVacancy(employeeId);
            var candidate = factory.CreateCandidate(vacancyId);
            var candidateData = candidate.Data!;
            var candidateRequest = factory.CandidateRequest!;
            Assert.That(candidateData.Id, Is.GreaterThan(0));
            Assert.That(candidateData.FirstName, Is.EqualTo(candidateRequest.FirstName));
            Assert.That(candidateData.MiddleName, Is.EqualTo(candidateRequest.MiddleName));
            Assert.That(candidateData.LastName, Is.EqualTo(candidateRequest.LastName));
            Assert.That(candidateData.Email, Is.EqualTo(candidateRequest.Email));

            var historyResponse = candidatesClient.GetCandidateHistory(candidateData.Id);

            Console.WriteLine($"Candidate history HTTP status: {(int)historyResponse.StatusCode}");
            Console.WriteLine($"Candidate history body: {historyResponse.Content}");

            Assert.That(historyResponse.IsSuccessful, Is.True,
                $"Candidate history request failed. " +
                $"HTTP: {(int)historyResponse.StatusCode}. " +
                $"Body: {historyResponse.Content}");

            Assert.That(historyResponse.Content, Is.Not.Null.And.Not.Empty,
                "Candidate history response body should not be empty.");

            using var document = JsonDocument.Parse(historyResponse.Content!);

            var root = document.RootElement;
            var historyData = root.GetProperty("data");
            var metadata = root.GetProperty("meta");

            Assert.That(metadata.GetProperty("total").GetInt32(), Is.EqualTo(2),
                "Expected two candidate history records.");

            Assert.That(historyData.GetArrayLength(), Is.EqualTo(2),
                "Expected two records in the candidate history data.");

            var historyRecords = historyData
                .EnumerateArray()
                .ToArray();

            JsonElement? applicationInitiatedRecord = historyRecords
                .Where(record =>
                    record.GetProperty("action")
                        .GetProperty("label")
                        .GetString() == "Application Initiated")
                .Select(record => (JsonElement?)record)
                .SingleOrDefault();

            JsonElement? addedRecord = historyRecords
                .Where(record =>
                    record.GetProperty("action")
                        .GetProperty("label")
                        .GetString() == "Added")
                .Select(record => (JsonElement?)record)
                .SingleOrDefault();

            Assert.That(applicationInitiatedRecord.HasValue, Is.True,
                "History should contain an Application Initiated record.");

            var applicationRecord = applicationInitiatedRecord!.Value;

            Assert.That(applicationRecord.GetProperty("id").GetInt32(), Is.GreaterThan(0));

            Assert.That(applicationRecord.GetProperty("vacancyName").GetString(), Is.EqualTo(factory.VacancyName),
                "Application Initiated should reference the created vacancy.");

            Assert.That(applicationRecord.GetProperty("performedDate").GetString(), Is.Not.Null.And.Not.Empty);

            Assert.That(addedRecord.HasValue, Is.True,
                "History should contain an Added record.");

            var addedRecordValue = addedRecord!.Value;

            Assert.That(addedRecordValue.GetProperty("id").GetInt32(), Is.GreaterThan(0));
            Assert.That(addedRecordValue.GetProperty("performedDate").GetString(), Is.Not.Null.And.Not.Empty);
            Assert.That(addedRecordValue.GetProperty("vacancyName").ValueKind, Is.EqualTo(JsonValueKind.Null),
                "Added record should not be associated with a vacancy.");
        }
        finally
        {
            TestDataCleanup.TryDelete(
                "candidate",
                factory.CandidateId,
                id => candidatesClient.DeleteCandidate(id));

            TestDataCleanup.TryDelete(
                "vacancy",
                factory.VacancyId,
                id => vacanciesClient.DeleteVacancy(id));

            TestDataCleanup.TryDelete(
                "employee",
                factory.EmployeeId,
                id => employeesClient.DeleteEmployee(id));
        }
    }
}