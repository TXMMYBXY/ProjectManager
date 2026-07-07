namespace ProjectManager.Entities.Models;

public class Document : EntityBase
{
    public string Title { get; set; }
    public string FilePath { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; }
}