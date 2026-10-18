using System.Text.Json.Serialization;

namespace OrangeHRM.Framework.Core.Api.Models.Responses;

public class ValidationErrorResponse
{
    [JsonPropertyName("error")]
    public ValidationErrorDetails Error { get; set; } = new();
}

public class ValidationErrorDetails
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public ValidationErrorData Data { get; set; } = new();
}

public class ValidationErrorData
{
    [JsonPropertyName("invalidParamKeys")]
    public List<string> InvalidParamKeys { get; set; } = new();
}