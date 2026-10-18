using System.Text.Json.Serialization;

namespace OrangeHRM.Framework.Core.Api.Models.Responses;

public class CreateVacancyResponse
{
    [JsonPropertyName("data")]
    public VacancyDto? Data { get; set; }

    [JsonPropertyName("meta")]
    public List<object> Meta { get; set; } = new();

    [JsonPropertyName("rels")]
    public List<object> Rels { get; set; } = new();
}

public class VacancyDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("numOfPositions")]
    public int? NumOfPositions { get; set; }

    [JsonPropertyName("status")]
    public bool Status { get; set; }

    [JsonPropertyName("isPublished")]
    public bool IsPublished { get; set; }

    [JsonPropertyName("jobTitle")]
    public VacancyJobTitleDto? JobTitle { get; set; }

    [JsonPropertyName("hiringManager")]
    public VacancyHiringManagerDto? HiringManager { get; set; }
}

public class VacancyJobTitleDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("isDeleted")]
    public bool IsDeleted { get; set; }
}

public class VacancyHiringManagerDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("middleName")]
    public string? MiddleName { get; set; }

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("terminationId")]
    public int? TerminationId { get; set; }
}