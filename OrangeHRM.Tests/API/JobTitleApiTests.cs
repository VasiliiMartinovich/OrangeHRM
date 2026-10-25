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
public class JobTitleApiTests : BaseApiTest
{
    [Test]
    [AllureName("POST Job Title - create job title and verify information")]
    [AllureTag("API", "JobTitles", "Smoke")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription("Creates a job title through the API, verifies the returned ID and job title information, and attempts to delete the created record.")]
    public void CreateJobTitle_ShouldReturnJobTitleInformation()
    {
        Log.Info("Starting test: CreateJobTitle_ShouldReturnJobTitleInformation.");

        using var client = new JobTitlesApiClient(AccessToken);

        int? jobTitleId = null;

        try
        {
            var jobTitleRequest = new CreateJobTitleRequest
            {
                Title = $"ApiTest Job {Guid.NewGuid().ToString("N")[..8]}",
                Description = "Job title created by API automation test",
                Note = "Created during automated testing"
            };

            var response = AllureApi.Step(
                "Create job title through API",
                () =>
                {
                    Log.Info($"Creating job title: {jobTitleRequest.Title}.");
                    return client.CreateJobTitle(jobTitleRequest);
                });

            Log.Info($"Create job title returned HTTP {(int)response.StatusCode}.");

            AllureApi.Step("Verify job title creation response", () =>
            {
                Assert.That(response.ErrorException, Is.Null,
                    "An exception occurred while sending the request.");

                Assert.That((int)response.StatusCode, Is.EqualTo(200),
                    $"Expected HTTP 200. Response: {response.Content}");

                Assert.That(response.Content, Is.Not.Null.And.Not.Empty,
                    "The response body was empty.");
            });

            var createdJobTitle = AllureApi.Step(
                "Deserialize created job title response",
                () =>
                {
                    return JsonSerializer.Deserialize<CreateJobTitleResponse>(response.Content!);
                });

            AllureApi.Step("Verify created job title information", () =>
            {
                Assert.That(createdJobTitle, Is.Not.Null,
                    "Could not deserialize the response.");

                Assert.That(createdJobTitle!.Data, Is.Not.Null,
                    "Job title data was missing from the response.");

                jobTitleId = createdJobTitle.Data!.Id;

                Assert.That(jobTitleId, Is.GreaterThan(0),
                    "Expected a positive job title ID.");

                Assert.That(createdJobTitle.Data.Title, Is.EqualTo(jobTitleRequest.Title));
                Assert.That(createdJobTitle.Data.Description, Is.EqualTo(jobTitleRequest.Description));
                Assert.That(createdJobTitle.Data.Note, Is.EqualTo(jobTitleRequest.Note));
            });

            Log.Info($"Job title created successfully. ID: {jobTitleId}.");
        }
        finally
        {
            AllureApi.Step("Clean up job title test data", () =>
            {
                if (jobTitleId.HasValue)
                {
                    TestDataCleanup.TryDelete(
                        "job title",
                        jobTitleId,
                        client.DeleteJobTitle);
                }
                else
                {
                    Log.Warn("Job title cleanup skipped because no ID was obtained.");
                }
            });
        }
    }
}