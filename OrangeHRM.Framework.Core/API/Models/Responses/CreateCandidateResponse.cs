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