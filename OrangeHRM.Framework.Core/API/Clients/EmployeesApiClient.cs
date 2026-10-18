using OrangeHRM.Framework.Core.Api.Models.Requests;
using RestSharp;

namespace OrangeHRM.Framework.Core.Api.Clients;

public class EmployeesApiClient : IDisposable
{
    private readonly RestClient _client;

    public EmployeesApiClient(string accessToken)
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

    public RestResponse GetEmployees()
    {
        var request = new RestRequest(
            "/web/index.php/api/v2/pim/employees",
            Method.Get);

        return _client.Execute(request);
    }
    
    public RestResponse CreateEmployee(CreateEmployeeRequest employee)
    {
        var request = new RestRequest(
            "/web/index.php/api/v2/pim/employees",
            Method.Post);

        request.AddJsonBody(employee);

        return _client.Execute(request);
    }

    public RestResponse DeleteEmployee(int employeeId)
    {
        var request = new RestRequest(
            "/web/index.php/api/v2/pim/employees",
            Method.Delete);

        request.AddJsonBody(new
        {
            ids = new[] { employeeId }
        });

        return _client.Execute(request);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}