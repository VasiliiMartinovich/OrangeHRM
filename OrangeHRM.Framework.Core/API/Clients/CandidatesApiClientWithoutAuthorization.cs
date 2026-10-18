using RestSharp;

namespace OrangeHRM.Framework.Core.Api.Clients;

public class CandidatesApiClientWithoutAuthorization : IDisposable
{
    private readonly RestClient _client;

    public CandidatesApiClientWithoutAuthorization()
    {
        var options = new RestClientOptions(
            "https://opensource-demo.orangehrmlive.com");

        _client = new RestClient(options);
    }

    public RestResponse GetCandidates()
    {
        var request = new RestRequest(
            "/web/index.php/api/v2/recruitment/candidates",
            Method.Get);

        return _client.Execute(request);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}