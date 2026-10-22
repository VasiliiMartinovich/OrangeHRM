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
public class CandidatesApiTests : BaseApiTest
{
    private static void RunStep(string name, Action action)
    {
        AllureApi.Step(name, action);
    }

    private static T RunStep<T>(string name, Func<T> action)
    {
        return AllureApi.Step(name, action);
    }

    [Test]
    [AllureName("GET Candidates - valid response structure")]
    [AllureTag("API", "Candidates")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Verifies that the candidates API returns HTTP 200 and a valid response structure containing candidate data, metadata, and relationship information.")]
    public void GetCandidates_ShouldReturnValidResponseStructure()
    {
        Log.Info("Starting test: GetCandidates_ShouldReturnValidResponseStructure.");

        using var client = new CandidatesApiClient(AccessToken);

        var response = RunStep(
            "Send GET request to retrieve candidates",
            () => client.GetCandidates());

        Log.Info($"GET candidates returned HTTP {(int)response.StatusCode}.");

        RunStep("Verify HTTP status code is 200", () =>
        {
            Assert.That((int)response.StatusCode, Is.EqualTo(200));
        });

        RunStep("Verify response body is not empty", () =>
        {
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
        });

        var result = RunStep(
            "Deserialize candidates response",
            () => JsonSerializer.Deserialize<CandidatesResponse>(
                response.Content!));

        RunStep("Verify candidates response structure", () =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Data, Is.Not.Null);
            Assert.That(result.Meta, Is.Not.Null);
            Assert.That(result.Meta!.Total, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.Rels, Is.Not.Null);
        });

        Log.Info(
            $"Test passed: candidates response structure is valid. Total: {result!.Meta!.Total}.");
    }

    [Test]
    [AllureName("GET Candidates - candidates contain required fields")]
    [AllureTag("API", "Candidates")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Verifies that the candidates API returns a successful response and that every returned candidate has a positive ID.")]
    public void GetCandidates_ShouldReturnCandidatesWithRequiredFields()
    {
        Log.Info("Starting test: GetCandidates_ShouldReturnCandidatesWithRequiredFields.");

        using var client = new CandidatesApiClient(AccessToken);

        var response = RunStep(
            "Send GET request to retrieve candidates",
            () => client.GetCandidates());

        RunStep("Verify HTTP status code is 200", () =>
        {
            Assert.That((int)response.StatusCode, Is.EqualTo(200));
        });

        RunStep("Verify response body is not empty", () =>
        {
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
        });

        var result = RunStep(
            "Deserialize candidates response",
            () => JsonSerializer.Deserialize<CandidatesResponse>(
                response.Content!));

        RunStep("Verify candidates are returned", () =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Data, Is.Not.Empty);
        });

        Log.Info($"Validating candidate IDs. Candidates returned: {result!.Data.Count}.");

        RunStep("Verify every candidate has a valid ID", () =>
        {
            foreach (var candidate in result.Data)
            {
                Assert.That(candidate.Id, Is.GreaterThan(0));
            }
        });

        Log.Info("Test passed: all returned candidates have valid IDs.");
    }

    [Test]
    [AllureName("GET Candidates - request without token is rejected")]
    [AllureTag("API", "Candidates")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Verifies that the candidates API rejects a request sent without an authorization token by returning HTTP 401 or 403.")]
    public void GetCandidates_WithoutToken_ShouldRejectRequest()
    {
        Log.Info("Starting test: GetCandidates_WithoutToken_ShouldRejectRequest.");

        using var client = new CandidatesApiClientWithoutAuthorization();

        var response = RunStep(
            "Send GET request without authorization",
            () => client.GetCandidates());

        Log.Info(
            $"Unauthorized candidates request returned HTTP {(int)response.StatusCode}.");

        RunStep("Verify the API rejects the unauthorized request", () =>
        {
            Assert.That(
                (int)response.StatusCode,
                Is.AnyOf(401, 403),
                "Expected the API to reject a request without authorization.");
        });

        Log.Info("Test passed: API rejected the request without authorization.");
    }

    private static void AssertStringProperty(
        JsonElement candidate,
        string propertyName,
        bool allowNull = false)
    {
        Assert.That(
            candidate.TryGetProperty(propertyName, out var property),
            Is.True,
            $"Candidate should contain {propertyName}.");

        var isString = property.ValueKind == JsonValueKind.String;
        var isAllowedNull = allowNull &&
                            property.ValueKind == JsonValueKind.Null;

        Assert.That(
            isString || isAllowedNull,
            Is.True,
            $"Property {propertyName} should be a string" +
            (allowNull ? " or null." : "."));
    }

    [Test]
    [AllureName("GET Candidates - invalid token does not return JSON")]
    [AllureTag("API", "Candidates")]
    [AllureSeverity(SeverityLevel.minor)]
    [AllureDescription(
        "Verifies that a request using an invalid authorization token does not return a response with the application/json content type.")]
    public void GetCandidates_WithInvalidToken_ShouldNotReturnJson()
    {
        Log.Info("Starting test: GetCandidates_WithInvalidToken_ShouldNotReturnJson.");

        using var client = new CandidatesApiClient("invalid-token");

        var response = RunStep(
            "Send GET request with an invalid token",
            () => client.GetCandidates());

        Log.Info(
            $"Invalid-token request returned HTTP {(int)response.StatusCode}. " +
            $"Content-Type: {response.ContentType ?? "not specified"}.");

        RunStep("Verify response Content-Type is not JSON", () =>
        {
            Assert.That(
                response.ContentType,
                Does.Not.Contain("application/json"),
                "Unexpected JSON response for an invalid token.");
        });

        Log.Info("Test passed: invalid-token response did not have JSON content type.");
    }

    [Test]
    [AllureName("GET Candidate - retrieve candidate by ID")]
    [AllureTag("API", "Candidates")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Creates a candidate, retrieves it by ID, and verifies that the response contains the expected candidate and matching first name, last name, and email. Deletes the test candidate during cleanup.")]
    public void GetCandidates_WithCandidateId_ShouldReturnMatchingCandidate()
    {
        Log.Info("Starting test: GetCandidates_WithCandidateId_ShouldReturnMatchingCandidate.");

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
            var createResponse = RunStep(
                "Create a candidate for the ID lookup test",
                () => client.CreateCandidate(candidateRequest));

            Log.Info(
                $"Create candidate returned HTTP {(int)createResponse.StatusCode}.");

            RunStep("Verify candidate creation succeeded", () =>
            {
                Assert.That(
                    createResponse.IsSuccessful,
                    Is.True,
                    $"Candidate creation failed. Status: {(int)createResponse.StatusCode}.");
            });

            var createdCandidate = RunStep(
                "Deserialize created candidate response",
                () => JsonSerializer.Deserialize<CreateCandidateResponse>(
                    createResponse.Content!));

            RunStep("Verify created candidate has an ID", () =>
            {
                Assert.That(createdCandidate?.Data, Is.Not.Null,
                    "The API did not return the created candidate.");
            });

            candidateId = createdCandidate!.Data!.Id;

            RunStep("Verify created candidate ID is valid", () =>
            {
                Assert.That(candidateId, Is.GreaterThan(0),
                    "The created candidate ID should be greater than zero.");
            });

            Log.Info($"Candidate created. Candidate ID: {candidateId.Value}.");

            var response = RunStep(
                "Get candidate by ID",
                () => client.GetCandidates(candidateId.Value));

            Log.Info(
                $"GET candidate by ID returned HTTP {(int)response.StatusCode}.");

            RunStep("Verify candidate lookup succeeded", () =>
            {
                Assert.That(
                    response.IsSuccessful,
                    Is.True,
                    $"Get candidate failed. Status: {(int)response.StatusCode}.");
            });

            var result = RunStep(
                "Deserialize candidate lookup response",
                () => JsonSerializer.Deserialize<CandidatesResponse>(
                    response.Content!));

            RunStep("Verify response contains the requested candidate", () =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result!.Data, Is.Not.Empty);

                Assert.That(
                    result.Data,
                    Has.All.Matches<CandidateDto>(
                        candidate => candidate.Id == candidateId.Value));

                Assert.That(
                    result.Data.Any(candidate =>
                        candidate.FirstName == candidateRequest.FirstName &&
                        candidate.LastName == candidateRequest.LastName &&
                        candidate.Email == candidateRequest.Email),
                    Is.True,
                    "The response did not contain the candidate that was created.");
            });

            Log.Info(
                $"Test passed: the response contains the expected candidate. ID: {candidateId.Value}.");
        }
        finally
        {
            RunStep("Clean up candidate created for the ID lookup test", () =>
            {
                Log.Info(
                    $"Starting candidate cleanup. Candidate ID: {candidateId?.ToString() ?? "not created"}.");

                TestDataCleanup.TryDelete(
                    "candidate",
                    candidateId,
                    client.DeleteCandidate);

                Log.Info("Candidate cleanup attempt completed.");
            });
        }
    }

    [Test]
    [AllureName("GET Candidate - non-existing ID returns empty list")]
    [AllureTag("API", "Candidates")]
    [AllureSeverity(SeverityLevel.minor)]
    [AllureDescription(
        "Verifies that querying the candidates API with a non-existing candidate ID returns HTTP 200 with an empty data list.")]
    public void GetCandidates_WithNonExistingCandidateId_ShouldReturnEmptyList()
    {
        Log.Info(
            "Starting test: GetCandidates_WithNonExistingCandidateId_ShouldReturnEmptyList.");

        using var client = new CandidatesApiClient(AccessToken);

        const int nonExistingCandidateId = 999999991;

        var response = RunStep(
            "Get candidate using a non-existing ID",
            () => client.GetCandidates(nonExistingCandidateId));

        Log.Info(
            $"GET candidate by non-existing ID returned HTTP {(int)response.StatusCode}.");

        RunStep("Verify request succeeded with HTTP 200", () =>
        {
            Assert.That((int)response.StatusCode, Is.EqualTo(200));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
        });

        var result = RunStep(
            "Deserialize response for non-existing candidate",
            () => JsonSerializer.Deserialize<CandidatesResponse>(
                response.Content!));

        RunStep("Verify no candidates were returned", () =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Data, Is.Empty,
                "Expected no candidates for a non-existing ID.");
        });

        Log.Info("Test passed: no candidates were returned for the non-existing ID.");
    }

    [Test]
    [AllureName("GET Candidates - non-existing name returns empty list")]
    [AllureTag("API", "Candidates")]
    [AllureSeverity(SeverityLevel.minor)]
    [AllureDescription(
        "Verifies that searching for a candidate name that does not exist returns HTTP 200 with an empty data list.")]
    public void GetCandidates_WithNonExistingCandidateName_ShouldReturnEmptyList()
    {
        Log.Info(
            "Starting test: GetCandidates_WithNonExistingCandidateName_ShouldReturnEmptyList.");

        using var client = new CandidatesApiClient(AccessToken);

        const string nonExistingCandidateName = "NoSuchCandidate_mIk_9018";

        var response = RunStep(
            "Search candidates by a non-existing name",
            () => client.GetCandidates(candidateName: nonExistingCandidateName));

        Log.Info(
            $"GET candidates by non-existing name returned HTTP {(int)response.StatusCode}.");

        RunStep("Verify search request succeeded", () =>
        {
            Assert.That((int)response.StatusCode, Is.EqualTo(200));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
        });

        var result = RunStep(
            "Deserialize candidate search response",
            () => JsonSerializer.Deserialize<CandidatesResponse>(
                response.Content!));

        RunStep("Verify search returned no candidates", () =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Data, Is.Empty,
                "Expected no candidates for a non-existing name.");
        });

        Log.Info("Test passed: no candidates were returned for the non-existing name.");
    }

    [Test]
    [AllureName("POST Candidate - create candidate successfully")]
    [AllureTag("API", "Candidates", "POST", "Positive", "Create")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Verifies that a candidate can be created through the API and that the returned ID, first name, last name, and email match the request.")]
    public void CreateCandidate_ShouldReturnCreatedCandidate()
    {
        Log.Info("Starting test: CreateCandidate_ShouldReturnCreatedCandidate.");

        using var client = new CandidatesApiClient(AccessToken);

        var candidate = new CreateCandidateRequest
        {
            FirstName = "ApiTest",
            LastName = "Candidate",
            Email = $"apitest.{Guid.NewGuid():N}@example.com"
        };

        var response = RunStep(
            "Send POST request to create a candidate",
            () => client.CreateCandidate(candidate));

        Log.Info(
            $"Create candidate returned HTTP {(int)response.StatusCode}.");

        RunStep("Verify candidate creation returned HTTP 200", () =>
        {
            Assert.That((int)response.StatusCode, Is.EqualTo(200));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
        });

        var result = RunStep(
            "Deserialize created candidate response",
            () => JsonSerializer.Deserialize<CreateCandidateResponse>(
                response.Content!));

        RunStep("Verify created candidate information", () =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Data, Is.Not.Null);
            Assert.That(result.Data!.Id, Is.GreaterThan(0));
            Assert.That(result.Data.FirstName, Is.EqualTo(candidate.FirstName));
            Assert.That(result.Data.LastName, Is.EqualTo(candidate.LastName));
            Assert.That(result.Data.Email, Is.EqualTo(candidate.Email));
        });

        Log.Info(
            $"Test passed: candidate created successfully. Candidate ID: {result!.Data!.Id}.");
    }

    [Test]
    [AllureName("POST Candidate - missing firstName returns validation error")]
    [AllureTag("API", "Candidates")]
    [AllureSeverity(SeverityLevel.minor)]
    [AllureDescription(
        "Verifies that creating a candidate without firstName returns HTTP 422 and identifies firstName in the validation error.")]
    public void CreateCandidate_WithoutFirstName_ShouldReturnValidationError()
    {
        Log.Info(
            "Starting test: CreateCandidate_WithoutFirstName_ShouldReturnValidationError.");

        using var client = new CandidatesApiClient(AccessToken);

        var candidate = new CreateCandidateRequest
        {
            LastName = "Candidate",
            Email = $"apitest.{Guid.NewGuid():N}@example.com"
        };

        var response = RunStep(
            "Create candidate without the firstName field",
            () => client.CreateCandidate(candidate));

        Log.Info(
            $"Create candidate without firstName returned HTTP {(int)response.StatusCode}.");

        RunStep("Verify HTTP 422 validation response", () =>
        {
            Assert.That((int)response.StatusCode, Is.EqualTo(422));
        });

        var error = RunStep(
            "Deserialize firstName validation error",
            () => JsonSerializer.Deserialize<ValidationErrorResponse>(
                response.Content!));

        RunStep("Verify firstName validation error details", () =>
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error!.Error.Status, Is.EqualTo("422"));
            Assert.That(error.Error.Message, Is.EqualTo("Invalid Parameter"));
            Assert.That(error.Error.Data.InvalidParamKeys, Does.Contain("firstName"));
        });

        Log.Info("Test passed: API returned the expected firstName validation error.");
    }

    [Test]
    [AllureName("POST Candidate - missing lastName returns validation error")]
    [AllureTag("API", "Candidates")]
    [AllureSeverity(SeverityLevel.minor)]
    [AllureDescription(
        "Verifies that creating a candidate without lastName returns HTTP 422 and identifies lastName in the validation error.")]
    public void CreateCandidate_WithoutLastName_ShouldReturnValidationError()
    {
        Log.Info(
            "Starting test: CreateCandidate_WithoutLastName_ShouldReturnValidationError.");

        using var client = new CandidatesApiClient(AccessToken);

        var candidate = new CreateCandidateRequest
        {
            FirstName = "ApiTest",
            Email = $"apitest.{Guid.NewGuid():N}@example.com"
        };

        var response = RunStep(
            "Create candidate without the lastName field",
            () => client.CreateCandidate(candidate));

        Log.Info(
            $"Create candidate without lastName returned HTTP {(int)response.StatusCode}.");

        RunStep("Verify HTTP 422 validation response", () =>
        {
            Assert.That((int)response.StatusCode, Is.EqualTo(422));
        });

        var error = RunStep(
            "Deserialize lastName validation error",
            () => JsonSerializer.Deserialize<ValidationErrorResponse>(
                response.Content!));

        RunStep("Verify lastName validation error details", () =>
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error!.Error.Status, Is.EqualTo("422"));
            Assert.That(error.Error.Message, Is.EqualTo("Invalid Parameter"));
            Assert.That(error.Error.Data.InvalidParamKeys, Does.Contain("lastName"));
        });

        Log.Info("Test passed: API returned the expected lastName validation error.");
    }

    [Test]
    [AllureName("POST Candidate - missing email returns validation error")]
    [AllureTag("API", "Candidates")]
    [AllureSeverity(SeverityLevel.minor)]
    [AllureDescription(
        "Verifies that creating a candidate without email returns HTTP 422 and identifies email in the validation error.")]
    public void CreateCandidate_WithoutEmailField_ShouldReturnValidationError()
    {
        Log.Info(
            "Starting test: CreateCandidate_WithoutEmailField_ShouldReturnValidationError.");

        using var client = new CandidatesApiClient(AccessToken);

        const string requestBody =
            """
            {
                "firstName": "ApiTest",
                "lastName": "Candidate"
            }
            """;

        var response = RunStep(
            "Create candidate without the email field",
            () => client.CreateCandidateWithoutEmailField(requestBody));

        Log.Info(
            $"Create candidate without email returned HTTP {(int)response.StatusCode}.");

        RunStep("Verify HTTP 422 validation response", () =>
        {
            Assert.That((int)response.StatusCode, Is.EqualTo(422));
        });

        var error = RunStep(
            "Deserialize email validation error",
            () => JsonSerializer.Deserialize<ValidationErrorResponse>(
                response.Content!));

        RunStep("Verify email validation error details", () =>
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error!.Error.Status, Is.EqualTo("422"));
            Assert.That(error.Error.Message, Is.EqualTo("Invalid Parameter"));
            Assert.That(error.Error.Data.InvalidParamKeys, Does.Contain("email"));
        });

        Log.Info("Test passed: API returned the expected email validation error.");
    }

    [Test]
    [AllureName("PUT Candidate - update candidate information")]
    [AllureTag("API", "Candidates")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Creates a candidate, updates its first name, middle name, last name, and email, verifies the changes through a subsequent GET request, and deletes the test candidate.")]
    public void UpdateCandidate_ShouldUpdateCandidateInformation()
    {
        Log.Info("Starting test: UpdateCandidate_ShouldUpdateCandidateInformation.");

        using var client = new CandidatesApiClient(AccessToken);

        var createRequest = new CreateCandidateRequest
        {
            FirstName = $"Put{Guid.NewGuid().ToString("N")[..8]}",
            MiddleName = $"Put{Guid.NewGuid().ToString("N")[..8]}",
            LastName = "BeforeUpdate",
            Email = $"put.{Guid.NewGuid():N}@example.com"
        };

        var createResponse = RunStep(
            "Create candidate before updating information",
            () => client.CreateCandidate(createRequest));

        Log.Info(
            $"Create candidate returned HTTP {(int)createResponse.StatusCode}.");

        RunStep("Verify candidate creation succeeded", () =>
        {
            Assert.That((int)createResponse.StatusCode, Is.EqualTo(200),
                "Candidate creation failed.");
        });

        var createdCandidate = RunStep(
            "Deserialize created candidate response",
            () => JsonSerializer.Deserialize<CreateCandidateResponse>(
                createResponse.Content!));

        RunStep("Verify created candidate response contains data", () =>
        {
            Assert.That(createdCandidate, Is.Not.Null);
            Assert.That(createdCandidate!.Data, Is.Not.Null);
        });

        var candidateId = createdCandidate!.Data!.Id;

        Log.Info($"Candidate created for update test. Candidate ID: {candidateId}.");

        try
        {
            var updateRequest = new UpdateCandidateRequest
            {
                FirstName = $"Updated{Guid.NewGuid().ToString("N")[..6]}",
                MiddleName = "AfterUpdate",
                LastName = "AfterUpdate",
                Email = $"updated.{Guid.NewGuid():N}@example.com"
            };

            var updateResponse = RunStep(
                "Update candidate information",
                () => client.UpdateCandidate(candidateId, updateRequest));

            Log.Info(
                $"Update candidate returned HTTP {(int)updateResponse.StatusCode}.");

            RunStep("Verify candidate update succeeded", () =>
            {
                Assert.That((int)updateResponse.StatusCode, Is.EqualTo(200),
                    "Candidate update failed.");
            });

            var getResponse = RunStep(
                "Get candidate after updating information",
                () => client.GetCandidates(candidateId: candidateId));

            Log.Info(
                $"GET candidate after update returned HTTP {(int)getResponse.StatusCode}.");

            RunStep("Verify GET after update succeeded", () =>
            {
                Assert.That((int)getResponse.StatusCode, Is.EqualTo(200),
                    "GET after update failed.");
            });

            var candidates = RunStep(
                "Deserialize candidate after update",
                () => JsonSerializer.Deserialize<CandidatesResponse>(
                    getResponse.Content!));

            RunStep("Verify updated candidate information", () =>
            {
                Assert.That(candidates, Is.Not.Null);
                Assert.That(candidates!.Data, Has.Count.EqualTo(1));

                var updatedCandidate = candidates.Data[0];

                Assert.That(updatedCandidate.FirstName, Is.EqualTo(updateRequest.FirstName));
                Assert.That(updatedCandidate.MiddleName, Is.EqualTo(updateRequest.MiddleName));
                Assert.That(updatedCandidate.LastName, Is.EqualTo(updateRequest.LastName));
                Assert.That(updatedCandidate.Email, Is.EqualTo(updateRequest.Email));
            });

            Log.Info(
                $"Test passed: candidate information was updated. Candidate ID: {candidateId}.");
        }
        finally
        {
            RunStep("Clean up candidate created for update test", () =>
            {
                Log.Info($"Starting cleanup for updated candidate. Candidate ID: {candidateId}.");

                var deleteResponse = client.DeleteCandidate(candidateId);

                Log.Info(
                    $"Cleanup DELETE returned HTTP {(int)deleteResponse.StatusCode}.");

                Assert.That((int)deleteResponse.StatusCode, Is.EqualTo(200),
                    "Failed to delete test candidate.");

                Log.Info($"Candidate cleanup completed. Candidate ID: {candidateId}.");
            });
        }
    }

    [Test]
    [AllureName("GET Candidates - search by candidate name")]
    [AllureTag("API", "Candidates")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Creates a candidate, searches by its generated first name, verifies that the candidate is included in the results, and deletes the test candidate.")]
    public void GetCandidates_WithCandidateName_ShouldReturnMatchingCandidates()
    {
        Log.Info(
            "Starting test: GetCandidates_WithCandidateName_ShouldReturnMatchingCandidates.");

        using var client = new CandidatesApiClient(AccessToken);

        var uniqueName = $"Api{Guid.NewGuid().ToString("N")[..8]}";

        var candidateRequest = new CreateCandidateRequest
        {
            FirstName = uniqueName,
            LastName = "SearchTest",
            Email = $"apitest.{Guid.NewGuid():N}@example.com"
        };

        var createResponse = RunStep(
            "Create candidate for name-search test",
            () => client.CreateCandidate(candidateRequest));

        Log.Info(
            $"Create candidate returned HTTP {(int)createResponse.StatusCode}.");

        RunStep("Verify candidate creation succeeded", () =>
        {
            Assert.That((int)createResponse.StatusCode, Is.EqualTo(200),
                "Candidate creation failed.");
        });

        var createdCandidate = RunStep(
            "Deserialize created candidate response",
            () => JsonSerializer.Deserialize<CreateCandidateResponse>(
                createResponse.Content!));

        RunStep("Verify created candidate response contains data", () =>
        {
            Assert.That(createdCandidate, Is.Not.Null);
            Assert.That(createdCandidate!.Data, Is.Not.Null);
        });

        var candidateId = createdCandidate!.Data!.Id;

        Log.Info($"Candidate created for search test. Candidate ID: {candidateId}.");

        try
        {
            var response = RunStep(
                "Search candidates by generated name",
                () => client.GetCandidates(candidateName: uniqueName));

            Log.Info(
                $"GET candidates by name returned HTTP {(int)response.StatusCode}.");

            RunStep("Verify name-search request succeeded", () =>
            {
                Assert.That((int)response.StatusCode, Is.EqualTo(200));
                Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
            });

            var result = RunStep(
                "Deserialize candidate name-search response",
                () => JsonSerializer.Deserialize<CandidatesResponse>(
                    response.Content!));

            RunStep("Verify search results contain the expected candidate", () =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result!.Data, Is.Not.Empty);

                Assert.That(
                    result.Data,
                    Has.All.Matches<CandidateDto>(
                        candidate => candidate.FirstName == uniqueName),
                    "Search returned candidates that do not match the requested name.");

                Assert.That(
                    result.Data,
                    Has.Some.Matches<CandidateDto>(
                        candidate => candidate.Id == candidateId),
                    "The newly created candidate was not found.");
            });

            Log.Info(
                $"Test passed: candidate was found by name. Candidate ID: {candidateId}.");
        }
        finally
        {
            RunStep("Clean up candidate created for name-search test", () =>
            {
                Log.Info($"Starting cleanup for search-test candidate. Candidate ID: {candidateId}.");

                var deleteResponse = client.DeleteCandidate(candidateId);

                Log.Info(
                    $"Cleanup DELETE returned HTTP {(int)deleteResponse.StatusCode}.");

                Assert.That((int)deleteResponse.StatusCode, Is.EqualTo(200),
                    "Failed to delete the test candidate.");

                Assert.That(deleteResponse.Content, Is.Not.Null.And.Not.Empty,
                    "Delete response body is empty.");

                var deletedCandidate =
                    JsonSerializer.Deserialize<DeleteCandidateResponse>(
                        deleteResponse.Content!);

                Assert.That(deletedCandidate, Is.Not.Null);
                Assert.That(
                    deletedCandidate!.Data,
                    Does.Contain(candidateId.ToString()),
                    "The API response does not contain the ID of the deleted candidate.");

                Log.Info($"Search-test candidate cleanup completed. Candidate ID: {candidateId}.");
            });
        }
    }

    [Test]
    [AllureName("DELETE Candidate - delete existing candidate")]
    [AllureTag("API", "Candidates")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Creates a candidate, deletes it, verifies the deletion response contains the candidate ID, and confirms the candidate is no longer returned by a subsequent GET request.")]
    public void DeleteCandidate_ShouldDeleteExistingCandidate()
    {
        Log.Info("Starting test: DeleteCandidate_ShouldDeleteExistingCandidate.");

        using var client = new CandidatesApiClient(AccessToken);

        var candidateRequest = new CreateCandidateRequest
        {
            FirstName = $"Del{Guid.NewGuid().ToString("N")[..8]}",
            LastName = "DeleteTest",
            Email = $"delete.{Guid.NewGuid():N}@example.com"
        };

        var createResponse = RunStep(
            "Create candidate for deletion test",
            () => client.CreateCandidate(candidateRequest));

        Log.Info(
            $"Create candidate returned HTTP {(int)createResponse.StatusCode}.");

        RunStep("Verify candidate creation succeeded", () =>
        {
            Assert.That((int)createResponse.StatusCode, Is.EqualTo(200),
                "Candidate creation failed.");
        });

        var createdCandidate = RunStep(
            "Deserialize created candidate response",
            () => JsonSerializer.Deserialize<CreateCandidateResponse>(
                createResponse.Content!));

        RunStep("Verify created candidate response contains data", () =>
        {
            Assert.That(createdCandidate, Is.Not.Null);
            Assert.That(createdCandidate!.Data, Is.Not.Null);
        });

        var candidateId = createdCandidate!.Data!.Id;

        Log.Info($"Candidate created for deletion test. Candidate ID: {candidateId}.");

        var deleteResponse = RunStep(
            "Delete candidate",
            () => client.DeleteCandidate(candidateId));

        Log.Info(
            $"Delete candidate returned HTTP {(int)deleteResponse.StatusCode}.");

        RunStep("Verify candidate deletion succeeded", () =>
        {
            Assert.That((int)deleteResponse.StatusCode, Is.EqualTo(200),
                "Candidate deletion failed.");
            Assert.That(deleteResponse.Content, Is.Not.Null.And.Not.Empty);
        });

        var deletedCandidate = RunStep(
            "Deserialize candidate deletion response",
            () => JsonSerializer.Deserialize<DeleteCandidateResponse>(
                deleteResponse.Content!));

        RunStep("Verify deletion response contains the candidate ID", () =>
        {
            Assert.That(deletedCandidate, Is.Not.Null);
            Assert.That(
                deletedCandidate!.Data,
                Does.Contain(candidateId.ToString()),
                "The delete response does not contain the ID of the deleted candidate.");
        });

        var getResponse = RunStep(
            "Verify candidate no longer exists",
            () => client.GetCandidates(candidateId: candidateId));

        Log.Info(
            $"GET candidate after deletion returned HTTP {(int)getResponse.StatusCode}.");

        RunStep("Verify lookup after deletion succeeds", () =>
        {
            Assert.That((int)getResponse.StatusCode, Is.EqualTo(200));
        });

        var candidatesAfterDeletion = RunStep(
            "Deserialize response after deletion",
            () => JsonSerializer.Deserialize<CandidatesResponse>(
                getResponse.Content!));

        RunStep("Verify candidate is absent from search results", () =>
        {
            Assert.That(candidatesAfterDeletion, Is.Not.Null);
            Assert.That(candidatesAfterDeletion!.Data, Is.Empty,
                "Candidate still exists after deletion.");
        });

        Log.Info(
            $"Test passed: candidate was deleted and no longer appears in search. ID: {candidateId}.");
    }

    [Test]
    [AllureName("DELETE Candidate - non-existing ID returns not found")]
    [AllureTag("API", "Candidates")]
    [AllureSeverity(SeverityLevel.minor)]
    [AllureDescription(
        "Verifies that deleting a candidate with a non-existing ID returns HTTP 404 and the expected Records Not Found error.")]
    public void DeleteCandidate_WithNonExistingId_ShouldReturnNotFound()
    {
        Log.Info(
            "Starting test: DeleteCandidate_WithNonExistingId_ShouldReturnNotFound.");

        using var client = new CandidatesApiClient(AccessToken);

        const int nonExistingCandidateId = 999999991;

        var response = RunStep(
            "Attempt to delete a non-existing candidate",
            () => client.DeleteCandidate(nonExistingCandidateId));

        Log.Info(
            $"Delete non-existing candidate returned HTTP {(int)response.StatusCode}.");

        RunStep("Verify API returns HTTP 404", () =>
        {
            Assert.That((int)response.StatusCode, Is.EqualTo(404));
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
        });

        var errorResponse = RunStep(
            "Deserialize candidate-not-found response",
            () => JsonSerializer.Deserialize<ValidationErrorResponse>(
                response.Content!));

        RunStep("Verify candidate-not-found error details", () =>
        {
            Assert.That(errorResponse, Is.Not.Null);
            Assert.That(errorResponse!.Error.Status, Is.EqualTo("404"));
            Assert.That(errorResponse.Error.Message, Is.EqualTo("Records Not Found"));
        });

        Log.Info("Test passed: API returned 404 for a non-existing candidate.");
    }

    private int CreateTestCandidate(CandidatesApiClient client)
    {
        var candidateId = RunStep(
            "Create test candidate for history-related tests",
            () =>
            {
                var request = new CreateCandidateRequest
                {
                    FirstName = $"History{Guid.NewGuid().ToString("N")[..8]}",
                    LastName = "ApiTest",
                    Email = $"history.{Guid.NewGuid():N}@example.com"
                };

                var response = client.CreateCandidate(request);

                Log.Info(
                    $"Helper candidate creation returned HTTP {(int)response.StatusCode}.");

                Assert.That((int)response.StatusCode, Is.EqualTo(200),
                    "Candidate creation failed.");

                var createdCandidate =
                    JsonSerializer.Deserialize<CreateCandidateResponse>(
                        response.Content!);

                Assert.That(createdCandidate, Is.Not.Null);
                Assert.That(createdCandidate!.Data, Is.Not.Null);

                return createdCandidate.Data!.Id;
            });

        Log.Info($"Helper created candidate. Candidate ID: {candidateId}.");

        return candidateId;
    }

    [Test]
    [AllureName("GET Candidate History - valid response")]
    [AllureTag("API", "Candidates")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Creates an employee, vacancy, and candidate, retrieves the candidate history, and verifies response metadata and the required fields of each history record. Attempts to clean up the test data afterward.")]
    public void GetCandidateHistory_ShouldReturnValidResponse()
    {
        Log.Info("Starting test: GetCandidateHistory_ShouldReturnValidResponse.");

        using var employeesClient = new EmployeesApiClient(AccessToken);
        using var vacanciesClient = new VacanciesApiClient(AccessToken);
        using var candidatesClient = new CandidatesApiClient(AccessToken);

        var factory = new CandidateVacancyTestDataFactory(
            employeesClient,
            vacanciesClient,
            candidatesClient);

        try
        {
            var employeeId = RunStep(
                "Create employee for candidate history test",
                () => factory.CreateEmployee());

            Log.Info($"Test employee created. ID: {employeeId}.");

            var vacancyId = RunStep(
                "Create vacancy for candidate history test",
                () => factory.CreateVacancy(employeeId));

            Log.Info($"Test vacancy created. ID: {vacancyId}.");

            var candidate = RunStep(
                "Create candidate associated with the vacancy",
                () => factory.CreateCandidate(vacancyId));

            var candidateId = candidate.Data!.Id;

            Log.Info($"Test candidate created. ID: {candidateId}.");

            var response = RunStep(
                "Get candidate history",
                () => candidatesClient.GetCandidateHistory(candidateId));

            Log.Info(
                $"GET candidate history returned HTTP {(int)response.StatusCode}.");

            RunStep("Verify candidate history request succeeded", () =>
            {
                Assert.That((int)response.StatusCode, Is.EqualTo(200),
                    "Candidate history request failed.");
                Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
            });

            var history = RunStep(
                "Deserialize candidate history response",
                () => JsonSerializer.Deserialize<CandidateHistoryResponse>(
                    response.Content!));

            RunStep("Verify candidate history response metadata", () =>
            {
                Assert.That(history, Is.Not.Null);
                Assert.That(history!.Data, Is.Not.Null);
                Assert.That(history.Meta, Is.Not.Null);
                Assert.That(history.Meta.Total, Is.GreaterThanOrEqualTo(0));
                Assert.That(history.Data.Count, Is.EqualTo(history.Meta.Total),
                    "The total count should match the number of returned history records.");
            });

            RunStep("Verify candidate history records", () =>
            {
                foreach (var item in history!.Data)
                {
                    Assert.That(item.Id, Is.GreaterThan(0));
                    Assert.That(item.Action, Is.Not.Null);
                    Assert.That(item.Action!.Id, Is.GreaterThan(0));
                    Assert.That(item.Action.Label, Is.Not.Empty);
                    Assert.That(item.PerformedBy, Is.Not.Null);
                    Assert.That(item.PerformedBy!.EmpNumber, Is.GreaterThan(0));
                    Assert.That(item.PerformedDate, Is.Not.Empty);
                }
            });

            Log.Info("Test passed: candidate history response is valid.");
        }
        finally
        {
            RunStep("Clean up candidate history test data", () =>
            {
                Log.Info("Starting cleanup of history-test data.");

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

                Log.Info("History-test data cleanup attempts completed.");
            });
        }
    }

    [Test]
    [AllureName("POST Candidate for Vacancy - candidate information and history")]
    [AllureTag("API", "Candidates")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription(
        "Creates an employee, vacancy, and candidate application, validates the candidate information and application history records, and attempts to clean up all created test data.")]
    public void CreateCandidateForVacancy_ShouldReturnCandidateInformationAndHistory()
    {
        Log.Info(
            "Starting test: CreateCandidateForVacancy_ShouldReturnCandidateInformationAndHistory.");

        using var employeesClient = new EmployeesApiClient(AccessToken);
        using var vacanciesClient = new VacanciesApiClient(AccessToken);
        using var candidatesClient = new CandidatesApiClient(AccessToken);

        var factory = new CandidateVacancyTestDataFactory(
            employeesClient,
            vacanciesClient,
            candidatesClient);

        try
        {
            var employeeId = RunStep(
                "Create employee for vacancy workflow",
                () => factory.CreateEmployee());

            Log.Info($"Test employee created. ID: {employeeId}.");

            var vacancyId = RunStep(
                "Create vacancy for candidate application",
                () => factory.CreateVacancy(employeeId));

            Log.Info($"Test vacancy created. ID: {vacancyId}.");

            var candidate = RunStep(
                "Create candidate for the vacancy",
                () => factory.CreateCandidate(vacancyId));

            var candidateData = candidate.Data!;
            var candidateRequest = factory.CandidateRequest!;

            Log.Info($"Candidate created for vacancy. Candidate ID: {candidateData.Id}.");

            RunStep("Verify created candidate information", () =>
            {
                Assert.That(candidateData.Id, Is.GreaterThan(0));
                Assert.That(candidateData.FirstName, Is.EqualTo(candidateRequest.FirstName));
                Assert.That(candidateData.MiddleName, Is.EqualTo(candidateRequest.MiddleName));
                Assert.That(candidateData.LastName, Is.EqualTo(candidateRequest.LastName));
                Assert.That(candidateData.Email, Is.EqualTo(candidateRequest.Email));
            });

            var historyResponse = RunStep(
                "Get history for the created candidate",
                () => candidatesClient.GetCandidateHistory(candidateData.Id));

            Log.Info(
                $"GET candidate history returned HTTP {(int)historyResponse.StatusCode}.");

            RunStep("Verify candidate history request succeeded", () =>
            {
                Assert.That(historyResponse.IsSuccessful, Is.True,
                    $"Candidate history request failed. HTTP: {(int)historyResponse.StatusCode}.");

                Assert.That(historyResponse.Content, Is.Not.Null.And.Not.Empty,
                    "Candidate history response body should not be empty.");
            });

            using var document = RunStep(
                "Parse candidate history JSON",
                () => JsonDocument.Parse(historyResponse.Content!));

            var root = document.RootElement;
            var historyData = root.GetProperty("data");
            var metadata = root.GetProperty("meta");

            RunStep("Verify candidate history contains two records", () =>
            {
                Assert.That(metadata.GetProperty("total").GetInt32(), Is.EqualTo(2),
                    "Expected two candidate history records.");

                Assert.That(historyData.GetArrayLength(), Is.EqualTo(2),
                    "Expected two records in the candidate history data.");
            });

            var historyRecords = historyData
                .EnumerateArray()
                .ToArray();

            JsonElement? applicationInitiatedRecord = RunStep(
                "Find Application Initiated history record",
                () => historyRecords
                    .Where(record =>
                        record.GetProperty("action")
                            .GetProperty("label")
                            .GetString() == "Application Initiated")
                    .Select(record => (JsonElement?)record)
                    .SingleOrDefault());

            JsonElement? addedRecord = RunStep(
                "Find Added history record",
                () => historyRecords
                    .Where(record =>
                        record.GetProperty("action")
                            .GetProperty("label")
                            .GetString() == "Added")
                    .Select(record => (JsonElement?)record)
                    .SingleOrDefault());

            RunStep("Verify Application Initiated history record", () =>
            {
                Assert.That(applicationInitiatedRecord.HasValue, Is.True,
                    "History should contain an Application Initiated record.");

                var applicationRecord = applicationInitiatedRecord!.Value;

                Assert.That(applicationRecord.GetProperty("id").GetInt32(), Is.GreaterThan(0));

                Assert.That(
                    applicationRecord.GetProperty("vacancyName").GetString(),
                    Is.EqualTo(factory.VacancyName),
                    "Application Initiated should reference the created vacancy.");

                Assert.That(
                    applicationRecord.GetProperty("performedDate").GetString(),
                    Is.Not.Null.And.Not.Empty);
            });

            RunStep("Verify Added history record", () =>
            {
                Assert.That(addedRecord.HasValue, Is.True,
                    "History should contain an Added record.");

                var addedRecordValue = addedRecord!.Value;

                Assert.That(addedRecordValue.GetProperty("id").GetInt32(), Is.GreaterThan(0));

                Assert.That(
                    addedRecordValue.GetProperty("performedDate").GetString(),
                    Is.Not.Null.And.Not.Empty);

                Assert.That(
                    addedRecordValue.GetProperty("vacancyName").ValueKind,
                    Is.EqualTo(JsonValueKind.Null),
                    "Added record should not be associated with a vacancy.");
            });

            Log.Info(
                $"Test passed: candidate information and history were verified. Candidate ID: {candidateData.Id}.");
        }
        finally
        {
            RunStep("Clean up candidate-vacancy workflow data", () =>
            {
                Log.Info("Starting cleanup of candidate-vacancy workflow data.");

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

                Log.Info("Candidate-vacancy workflow cleanup attempts completed.");
            });
        }
    }
}