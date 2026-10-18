using System.Text.Json.Serialization;

namespace OrangeHRM.Framework.Core.Api.Models.Requests;

public class UpdateCandidateRequest
{
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }
    
    [JsonPropertyName("middleName")]
    public string MiddleName { get; set; } 

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } 

    [JsonPropertyName("email")]
    public string Email { get; set; } 
}