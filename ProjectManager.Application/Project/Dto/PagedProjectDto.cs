using ProjectManager.Application.Common;

namespace ProjectManager.Application.Project.Dto;

public class PagedProjectDto : PagedData
{
    public IReadOnlyList<ProjectItemDto>? Projects { get; set; }
}