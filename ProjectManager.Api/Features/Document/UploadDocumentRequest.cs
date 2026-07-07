using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjectManager.Api.Features.Document;

public class UploadDocumentRequest
{
    [Required]
    [JsonPropertyName("file")]
    public IFormFile File { get; set; }
}