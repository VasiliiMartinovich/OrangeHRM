using System.Text.Json.Serialization;

namespace OrangeHRM.Framework.Core.Api.Models.Requests;

public class CreateJobTitleRequest
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("note")]
    public string Note { get; set; } = string.Empty;
}