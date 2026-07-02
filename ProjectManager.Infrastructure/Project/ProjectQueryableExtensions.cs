using ProjectManager.Application.Project;
using ProjectManager.Application.Project.Dto;

namespace ProjectManager.Infrastructure.Project;

public static class ProjectQueryableExtensions
{
    public static IQueryable<Entities.Models.Project> ApplyFilter(this IQueryable<Entities.Models.Project> query,
        ProjectFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Title))
            query = query.Where(p => p.Title.Contains(filter.Title));

        if (!string.IsNullOrWhiteSpace(filter.CompanyCustomer))
            query = query.Where(p => p.CompanyCustomer.Contains(filter.CompanyCustomer));
        
        if (!string.IsNullOrWhiteSpace(filter.CompanyExecuter))
            query = query.Where(p => p.CompanyExecuter.Contains(filter.CompanyExecuter));
        
        if(filter.StartDateFrom.HasValue)
            query = query.Where(p => p.StartDate <= filter.StartDateFrom.Value);
        
        if(filter.StartDateTo.HasValue)
            query = query.Where(p => p.StartDate >= filter.StartDateTo.Value);
        
        if(filter.FinishDateFrom.HasValue)
            query = query.Where(p => p.FinishDate <= filter.FinishDateFrom.Value);
        
        if(filter.FinishDateTo.HasValue)
            query = query.Where(p => p.FinishDate >= filter.FinishDateTo.Value);

        if (filter.Priority.HasValue)
            query = query.Where(p => p.Priority == filter.Priority.Value);

        if (filter.ProjectManagerId.HasValue)
            query = query.Where(p => p.ProjectManagerId == filter.ProjectManagerId.Value);
        
        return query;
    }
    
    public static IQueryable<Entities.Models.Project> ApplySorting(this IQueryable<Entities.Models.Project> query,
        ProjectFilter filter)
    {
        if (filter.SortBy.HasValue)
        {
            query = filter.SortBy.Value switch
            {
                ProjectSortField.Title =>
                    filter.Descending
                        ? query.OrderBy(p => p.Title)
                        : query.OrderByDescending(p => p.Title),

                ProjectSortField.CompanyCustomer =>
                    filter.Descending
                        ? query.OrderBy(p => p.CompanyCustomer)
                        : query.OrderByDescending(p => p.CompanyCustomer),

                ProjectSortField.CompanyExecuter =>
                    filter.Descending
                        ? query.OrderBy(p => p.CompanyExecuter)
                        : query.OrderByDescending(p => p.CompanyExecuter),

                ProjectSortField.StartDate =>
                    filter.Descending
                        ? query.OrderBy(p => p.StartDate)
                        : query.OrderByDescending(p => p.StartDate),

                ProjectSortField.FinishDate =>
                    filter.Descending
                        ? query.OrderBy(p => p.FinishDate)
                        : query.OrderByDescending(p => p.FinishDate),

                ProjectSortField.Priority =>
                    filter.Descending
                        ? query.OrderBy(p => p.Priority)
                        : query.OrderByDescending(p => p.Priority),

                ProjectSortField.ProjectManagerLastName =>
                    filter.Descending
                        ? query.OrderBy(p => p.ProjectManager!.LastName)
                        : query.OrderByDescending(p => p.ProjectManager!.LastName),
                
                _ => filter.Descending
                    ? query.OrderBy(p => p.Title)
                    : query.OrderByDescending(p => p.Title)
            };
        }
        
        return query;
    }
    
    public static IQueryable<ProjectItemDto> ApplyPagination(this IQueryable<ProjectItemDto> query,
        ProjectFilter filter)
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