using System.Text.Json.Serialization;

namespace OrangeHRM.Framework.Core.Api.Models.Responses;

public class CandidatesResponse
{
    [JsonPropertyName("data")]
    public List<CandidateDto> Data { get; set; } = new();

    [JsonPropertyName("meta")]
    public CandidatesMetaDto? Meta { get; set; }

    [JsonPropertyName("rels")]
    public List<object> Rels { get; set; } = new();
}

public class CandidateDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [JsonPropertyName("middleName")]
    public string? MiddleName { get; set; }

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } 

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}

public class CandidatesMetaDto
{
    [JsonPropertyName("total")]
    public int Total { get; set; }
}