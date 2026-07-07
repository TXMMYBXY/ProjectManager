namespace ProjectManager.Application.Document.Dto;

public class UploadDocumentDto
{
    public string FileName { get; set; }
    public long FileLength { get; set; }
    public Stream FileStream { get; set; }
}