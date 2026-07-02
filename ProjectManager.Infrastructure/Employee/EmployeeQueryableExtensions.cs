using ProjectManager.Application.Employee;
using ProjectManager.Application.Employee.Dto;

namespace ProjectManager.Infrastructure.Employee;

public static class EmployeeQueryableExtensions
{
    public static IQueryable<Entities.Models.Employee> ApplyFilter(this IQueryable<Entities.Models.Employee> query,
        EmployeeFilter filter)
    {
        if(!string.IsNullOrWhiteSpace(filter.FirstName))
            query = query.Where(e => e.FirstName.Contains(filter.FirstName));
        
        if(!string.IsNullOrWhiteSpace(filter.LastName))
            query = query.Where(e => e.LastName.Contains(filter.LastName));

        if(!string.IsNullOrWhiteSpace(filter.Patronymic))
            query = query.Where(e => e.Patronymic.Contains(filter.Patronymic));
        
        if(!string.IsNullOrWhiteSpace(filter.Email))
            query = query.Where(e => e.Email.Contains(filter.Email));
        
        return query;
    }

    public static IQueryable<Entities.Models.Employee> ApplySorting(this IQueryable<Entities.Models.Employee> query, 
        EmployeeFilter filter)
    {
        if (filter.SortField.HasValue)
        {
            query = filter.SortField switch
            {
                EmployeeSortField.FirstName =>
                    filter.Descending
                        ? query.OrderBy(e => e.FirstName)
                        : query.OrderByDescending(e => e.FirstName),

                EmployeeSortField.LastName =>
                    filter.Descending
                        ? query.OrderBy(e => e.LastName)
                        : query.OrderByDescending(e => e.LastName),

                EmployeeSortField.Patronymic =>
                    filter.Descending
                        ? query.OrderBy(e => e.Patronymic)
                        : query.OrderByDescending(e => e.Patronymic),

                EmployeeSortField.Email =>
                    filter.Descending
                        ? query.OrderBy(e => e.Email)
                        : query.OrderByDescending(e => e.Email),

                _ => 
                    filter.Descending
                        ? query.OrderBy(e => e.LastName)
                        : query.OrderByDescending(e => e.LastName)
            };
        }
        
        return query;
    }

    public static IQueryable<EmployeeItemDto> ApplyPagination(this IQueryable<EmployeeItemDto> query,
        EmployeeFilter filter)
    {
        if (filter.PageSize.HasValue && filter.PageNumber.HasValue)
        {
            query = query
                .Skip((filter.PageNumber.Value - 1) * filter.PageSize.Value)
                .Take(filter.PageSize.Value);
        }
        
        return query;
    }
}