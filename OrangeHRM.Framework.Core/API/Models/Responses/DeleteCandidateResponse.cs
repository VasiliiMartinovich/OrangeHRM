using System.Text.Json.Serialization;

namespace OrangeHRM.Framework.Core.Api.Models.Responses;

public class DeleteCandidateResponse
{
    [JsonPropertyName("data")]
    public List<string> Data { get; set; } = new();

    [JsonPropertyName("meta")]
    public List<object> Meta { get; set; } = new();

    [JsonPropertyName("rels")]
    public List<object> Rels { get; set; } = new();
}