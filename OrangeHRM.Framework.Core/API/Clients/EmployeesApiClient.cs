using System.Diagnostics;
using log4net;
using OrangeHRM.Framework.Core.Api.Models.Requests;
using RestSharp;

namespace OrangeHRM.Framework.Core.Api.Clients;

public class EmployeesApiClient : IDisposable
{
    private readonly RestClient _client;
    private readonly ILog _log = LogManager.GetLogger(typeof(EmployeesApiClient));

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

        // Токен намеренно не записывается в лог.
        _log.Info("Employees API client initialized.");
    }

    public RestResponse GetEmployees()
    {
        var request = new RestRequest(
            "/web/index.php/api/v2/pim/employees",
            Method.Get);

        return ExecuteWithLogging(request);
    }

    public RestResponse CreateEmployee(CreateEmployeeRequest employee)
    {
        ArgumentNullException.ThrowIfNull(employee);

        var request = new RestRequest(
            "/web/index.php/api/v2/pim/employees",
            Method.Post);

        request.AddJsonBody(employee);

        return ExecuteWithLogging(request);
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

        return ExecuteWithLogging(request);
    }

    private RestResponse ExecuteWithLogging(RestRequest request)
    {
        var stopwatch = Stopwatch.StartNew();

        _log.Info(
            $"Sending API request: {request.Method} {request.Resource}");

        try
        {
            var response = _client.Execute(request);

            stopwatch.Stop();

            var statusCode = (int)response.StatusCode;

            if (response.IsSuccessful)
            {
                _log.Info(
                    $"API request completed: {request.Method} {request.Resource}. " +
                    $"HTTP status: {statusCode}. " +
                    $"Duration: {stopwatch.ElapsedMilliseconds} ms.");
            }
            else
            {
                _log.Warn(
                    $"API request was unsuccessful: {request.Method} {request.Resource}. " +
                    $"HTTP status: {statusCode}. " +
                    $"Response status: {response.ResponseStatus}. " +
                    $"Duration: {stopwatch.ElapsedMilliseconds} ms.");
            }

            if (response.ErrorException != null)
            {
                _log.Error(
                    $"API transport error: {request.Method} {request.Resource}.",
                    response.ErrorException);
            }

            return response;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            _log.Error(
                $"Exception while executing API request: " +
                $"{request.Method} {request.Resource}. " +
                $"Duration before failure: {stopwatch.ElapsedMilliseconds} ms.",
                exception);

            throw;
        }
    }

    public void Dispose()
    {
        _log.Info("Disposing Employees API client.");
        _client.Dispose();
    }
}
