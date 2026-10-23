using System.Text.Json.Serialization;

namespace OrangeHRM.Framework.Core.Api.Models.Responses;

public class CreateJobTitleResponse
{
    [JsonPropertyName("data")]
    public JobTitleDto? Data { get; set; }
}

public class JobTitleDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }
}