using System.Text.Json.Serialization;

namespace OrangeHRM.Framework.Core.Api.Models.Requests;

public class CreateCandidateRequest
{
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [JsonPropertyName("middleName")]
    public string MiddleName { get; set; } 
    
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } 

    [JsonPropertyName("email")]
    public string Email { get; set; } 
    
    [JsonPropertyName("contactNumber")]
    public string ContactNumber { get; set; } = string.Empty;

    [JsonPropertyName("vacancyId")]
    public int? VacancyId { get; set; }

    [JsonPropertyName("keywords")]
    public string Keywords { get; set; } = string.Empty;

    [JsonPropertyName("comment")]
    public string Comment { get; set; } = string.Empty;

    [JsonPropertyName("dateOfApplication")]
    public string? DateOfApplication { get; set; }

    [JsonPropertyName("consentToKeepData")]
    public bool ConsentToKeepData { get; set; }
}