using ProjectManager.Application.Common;

namespace ProjectManager.Application.Employee.Dto;

public class PagedEmployeeDto : PagedData
{
    public IReadOnlyList<EmployeeItemDto> Employees { get; set; }
}