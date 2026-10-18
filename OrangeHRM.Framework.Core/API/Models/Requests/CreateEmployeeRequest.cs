using System.Text.Json.Serialization;

namespace OrangeHRM.Framework.Core.Api.Models.Requests;

public class CreateEmployeeRequest
{
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("middleName")]
    public string MiddleName { get; set; } = string.Empty;

    [JsonPropertyName("employeeId")]
    public string? EmployeeId { get; set; }
}