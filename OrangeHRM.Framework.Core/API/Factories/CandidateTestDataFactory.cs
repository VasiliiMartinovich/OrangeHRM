using OrangeHRM.Framework.Core.Api.Clients;
using OrangeHRM.Framework.Core.Api.Models.Requests;
using OrangeHRM.Framework.Core.Api.Models.Responses;
using System.Text.Json;

namespace OrangeHRM.Framework.Core.Api.Factories;

public class CandidateTestDataFactory
{
    private readonly CandidatesApiClient _client;

    public CandidateTestDataFactory(CandidatesApiClient client)
    {
        _client = client;
    }

    public int CreateCandidate()
    {
        var request = new CreateCandidateRequest
        {
            FirstName = $"History{Guid.NewGuid().ToString("N")[..8]}",
            LastName = "ApiTest",
            Email = $"history.{Guid.NewGuid():N}@example.com"
        };

        var response = _client.CreateCandidate(request);

        if ((int)response.StatusCode != 200)
        {
            throw new InvalidOperationException(
                $"Candidate creation failed. " +
                $"Status: {(int)response.StatusCode}. " +
                $"Body: {response.Content}");
        }

        var createdCandidate = JsonSerializer.Deserialize<CreateCandidateResponse>(response.Content!);

        if (createdCandidate?.Data == null)
        {
            throw new InvalidOperationException(
                $"Candidate was created, but its ID was not returned. " +
                $"Body: {response.Content}");
        }

        return createdCandidate.Data.Id;
    }
}