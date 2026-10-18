using System.Text.Json.Serialization;

namespace OrangeHRM.Framework.Core.Api.Models.Responses;

public class CreateEmployeeResponse
{
    [JsonPropertyName("data")]
    public EmployeeDto? Data { get; set; }

    [JsonPropertyName("meta")]
    public List<object> Meta { get; set; } = new();

    [JsonPropertyName("rels")]
    public List<object> Rels { get; set; } = new();
}

public class EmployeeDto
{
    [JsonPropertyName("empNumber")]
    public int EmpNumber { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("middleName")]
    public string? MiddleName { get; set; }

    [JsonPropertyName("employeeId")]
    public string? EmployeeId { get; set; }

    [JsonPropertyName("terminationId")]
    public int? TerminationId { get; set; }
}