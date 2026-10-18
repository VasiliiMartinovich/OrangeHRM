using System.Text.Json;
using NUnit.Framework;
using OrangeHRM.Framework.Core.Api.Models.Responses;
using RestSharp;
using OrangeHRM.Framework.Core.Api.Models.Requests;

namespace OrangeHRM.Framework.Core.Api.Clients;

public class CandidatesApiClient : IDisposable
{
    private readonly RestClient _client;

    public CandidatesApiClient(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new ArgumentException(
                "Access token cannot be empty.",
                nameof(accessToken));
        }

        var options = new RestClientOptions(
            "https://opensource-demo.orangehrmlive.com");

        _client = new RestClient(options);

        _client.AddDefaultHeader(
            "Authorization",
            $"Bearer {accessToken}");
    }

    public RestResponse GetCandidates(
        int? candidateId = null,
        string? candidateName = null)
    {
        var request = new RestRequest(
            "/web/index.php/api/v2/recruitment/candidates",
            Method.Get);

        if (candidateId.HasValue)
        {
            request.AddQueryParameter(
                "candidateId",
                candidateId.Value);
        }

        if (!string.IsNullOrWhiteSpace(candidateName))
        {
            request.AddQueryParameter(
                "candidateName",
                candidateName);
        }

        return _client.Execute(request);
    }

    public RestResponse CreateCandidate(CreateCandidateRequest candidate)
    {
        var request = new RestRequest("/web/index.php/api/v2/recruitment/candidates",
            Method.Post);

        request.AddHeader("Content-Type", "application/json");

        request.AddStringBody(JsonSerializer.Serialize(candidate), DataFormat.Json);

        return _client.Execute(request);
    }

    public RestResponse CreateCandidateWithoutEmailField(
        string requestBody)
    {
        var request = new RestRequest(
            "/web/index.php/api/v2/recruitment/candidates",
            Method.Post);

        request.AddStringBody(
            requestBody,
            DataFormat.Json);

        return _client.Execute(request);
    }
    
    public RestResponse UpdateCandidate(
        int candidateId,
        UpdateCandidateRequest candidate)
    {
        var request = new RestRequest(
            $"/web/index.php/api/v2/recruitment/candidates/{candidateId}",
            Method.Put);

        request.AddJsonBody(candidate);

        return _client.Execute(request);
    }
    
    public RestResponse DeleteCandidate(int candidateId)
    {
        var request = new RestRequest(
            "/web/index.php/api/v2/recruitment/candidates",
            Method.Delete);

        request.AddJsonBody(new
        {
            ids = new[] { candidateId }
        });

        return _client.Execute(request);
    }
    
    public RestResponse GetCandidateHistory(
        int candidateId,
        int? limit = null,
        int? offset = null,
        string? sortOrder = null)
    {
        var request = new RestRequest(
            $"/web/index.php/api/v2/recruitment/candidates/{candidateId}/history",
            Method.Get);

        if (limit.HasValue)
        {
            request.AddQueryParameter("limit", limit.Value);
        }

        if (offset.HasValue)
        {
            request.AddQueryParameter("offset", offset.Value);
        }

        if (!string.IsNullOrWhiteSpace(sortOrder))
        {
            request.AddQueryParameter("sortOrder", sortOrder);
        }

        return _client.Execute(request);
    }
    
    public void Dispose()
    {
        _client.Dispose();
    }
}