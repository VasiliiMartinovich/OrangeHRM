using System.Text.Json.Serialization;

namespace OrangeHRM.Framework.Core.Api.Models.Requests;

public class CreateVacancyRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public bool Status { get; set; }

    [JsonPropertyName("jobTitleId")]
    public int JobTitleId { get; set; }

    [JsonPropertyName("isPublished")]
    public bool IsPublished { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("numOfPositions")]
    public int? NumOfPositions { get; set; }

    [JsonPropertyName("employeeId")]
    public int EmployeeId { get; set; }
}