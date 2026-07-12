using ProjectManager.Application.Common;
using ProjectManager.Application.Utils;

namespace ProjectManager.Application.Project.Dto;

public class PagedProjectDto : PagedData
{
    public IReadOnlyList<ProjectItemDto>? Projects { get; set; }
}