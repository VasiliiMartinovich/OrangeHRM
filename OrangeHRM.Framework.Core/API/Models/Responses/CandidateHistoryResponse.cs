using System.Text.Json.Serialization;

namespace OrangeHRM.Framework.Core.Api.Models.Responses;

public class CandidateHistoryResponse
{
    [JsonPropertyName("data")]
    public List<CandidateHistoryItemDto> Data { get; set; } = new();

    [JsonPropertyName("meta")]
    public CandidateHistoryMetaDto Meta { get; set; } = new();

    [JsonPropertyName("rels")]
    public List<object> Rels { get; set; } = new();
}

public class CandidateHistoryMetaDto
{
    [JsonPropertyName("total")]
    public int Total { get; set; }
}

public class CandidateHistoryItemDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("action")]
    public CandidateHistoryActionDto Action { get; set; } = new();

    [JsonPropertyName("vacancyName")]
    public string? VacancyName { get; set; }

    [JsonPropertyName("performedBy")]
    public CandidateHistoryEmployeeDto PerformedBy { get; set; } = new();

    [JsonPropertyName("performedDate")]
    public string PerformedDate { get; set; } = string.Empty;

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonPropertyName("editable")]
    public bool Editable { get; set; }

    [JsonPropertyName("interview")]
    public CandidateHistoryInterviewDto? Interview { get; set; }
}

public class CandidateHistoryActionDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;
}

public class CandidateHistoryEmployeeDto
{
    [JsonPropertyName("empNumber")]
    public int EmpNumber { get; set; }

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("middleName")]
    public string? MiddleName { get; set; }

    [JsonPropertyName("terminationId")]
    public int? TerminationId { get; set; }
}

public class CandidateHistoryInterviewDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("time")]
    public string? Time { get; set; }

    [JsonPropertyName("interviewers")]
    public List<CandidateHistoryInterviewerDto> Interviewers { get; set; } = new();
}

public class CandidateHistoryInterviewerDto
{
    [JsonPropertyName("empNumber")]
    public int EmpNumber { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("middleName")]
    public string? MiddleName { get; set; }

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("terminationId")]
    public int? TerminationId { get; set; }
}