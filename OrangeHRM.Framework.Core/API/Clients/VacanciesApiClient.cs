using OrangeHRM.Framework.Core.Api.Models.Requests;
using RestSharp;

namespace OrangeHRM.Framework.Core.Api.Clients;

public class VacanciesApiClient : IDisposable
{
    private readonly RestClient _client;

    public VacanciesApiClient(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new ArgumentException("Access token cannot be empty.", nameof(accessToken));
        }

        _client = new RestClient(new RestClientOptions("https://opensource-demo.orangehrmlive.com"));
        _client.AddDefaultHeader("Authorization", $"Bearer {accessToken}");
    }

    public RestResponse CreateVacancy(CreateVacancyRequest vacancy)
    {
        var request = new RestRequest("/web/index.php/api/v2/recruitment/vacancies",
            Method.Post);

        request.AddJsonBody(vacancy);

        return _client.Execute(request);
    }

    public RestResponse DeleteVacancy(int vacancyId)
    {
        var request = new RestRequest("/web/index.php/api/v2/recruitment/vacancies",
            Method.Delete);

        request.AddJsonBody(new
        {
            ids = new[] { vacancyId }
        });

        return _client.Execute(request);
    }
    
    public void Dispose()
    {
        _client.Dispose();
    }
}