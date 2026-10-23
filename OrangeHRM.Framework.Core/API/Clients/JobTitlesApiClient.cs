using System.Text.Json;
using NUnit.Framework;
using OrangeHRM.Framework.Core.Api.Models.Responses;
using RestSharp;
using OrangeHRM.Framework.Core.Api.Models.Requests;

namespace OrangeHRM.Framework.Core.Api.Clients;

public class JobTitlesApiClient : IDisposable
{
    private readonly RestClient _client;

    public JobTitlesApiClient(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new ArgumentException("Access token cannot be empty.", nameof(accessToken));
        }

        var options = new RestClientOptions("https://opensource-demo.orangehrmlive.com");

        _client = new RestClient(options);

        _client.AddDefaultHeader("Authorization", $"Bearer {accessToken}");
    }

    // public RestResponse CreateJobTitle(CreateJobTitleRequest jobTitle)
    // {
    //     var request = new RestRequest("/web/index.php/api/v2/admin/job-titles",
    //         Method.Post);
    //
    //     request.AddStringBody(JsonSerializer.Serialize(jobTitle), DataFormat.Json);
    //
    //     return _client.Execute(request);
    // }
    public RestResponse CreateJobTitle(CreateJobTitleRequest jobTitle)
    {
        var request = new RestRequest(
            "/web/index.php/api/v2/admin/job-titles",
            Method.Post);

        var json = JsonSerializer.Serialize(jobTitle);

        TestContext.WriteLine($"Job Title request JSON: {json}");

        request.AddStringBody(json, DataFormat.Json);

        return _client.Execute(request);
    }
    
    public RestResponse DeleteJobTitle(int jobTitleId)
    {
        var request = new RestRequest(
            "/web/index.php/api/v2/admin/job-titles",
            Method.Delete);

        request.AddJsonBody(new
        {
            ids = new[] { jobTitleId }
        });

        return _client.Execute(request);
    }
    
    public void Dispose()
    {
        _client.Dispose();
    }
}