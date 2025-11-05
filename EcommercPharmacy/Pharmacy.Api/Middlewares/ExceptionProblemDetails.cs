using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Pharmacy.Api.Middlewares
{
    public class ExceptionProblemDetails : ProblemDetails
    {
        public bool Success { get; set; } = false;
        public string? ErrorCode { get; set; }
        public string? ErrorSource { get; set; }
        public string? Timestamp { get; set; } 

        [JsonInclude]
        [JsonPropertyName("errors")]
        public object? Errors { get; set; }
    }
}
