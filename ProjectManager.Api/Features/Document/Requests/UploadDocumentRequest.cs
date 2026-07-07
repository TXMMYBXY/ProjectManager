using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjectManager.Api.Features.Document.Requests;

public class UploadDocumentRequest
{
    [Required]
    [JsonPropertyName("file")]
    public IFormFile File { get; set; }
}