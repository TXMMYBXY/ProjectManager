using ProjectManager.Application.Common;
using ProjectManager.Application.Utils;

namespace ProjectManager.Application.Employee.Dto;

public class PagedEmployeeDto : PagedData
{
    public IReadOnlyList<EmployeeItemDto> Employees { get; set; }
}