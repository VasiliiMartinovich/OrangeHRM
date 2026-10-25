using System.Text.Json.Serialization;

namespace OrangeHRM.Framework.Core.Api.Models.Responses;

public class CreateCandidateResponse
{
    [JsonPropertyName("data")]
    public CandidateDto? Data { get; set; }

    [JsonPropertyName("meta")]
    public List<object> Meta { get; set; } = new();

    [JsonPropertyName("rels")]
    public List<object> Rels { get; set; } = new();
}

// public class CreatedCandidateDto
// {
//     [JsonPropertyName("id")]
//     public int Id { get; set; }
//
//     [JsonPropertyName("firstName")]
//     public string FirstName { get; set; } = string.Empty;
//
//     [JsonPropertyName("middleName")]
//     public string? MiddleName { get; set; }
//
//     [JsonPropertyName("lastName")]
//     public string LastName { get; set; } = string.Empty;
//
//     [JsonPropertyName("email")]
//     public string Email { get; set; } = string.Empty;
// }