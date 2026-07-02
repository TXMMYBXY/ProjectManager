using ProjectManager.Application.Common;

namespace ProjectManager.Application.Project.Dto;

public class PagedProjectDto : PagedData
{
    public ICollection<ProjectItemDto>? Projects { get; set; }
}