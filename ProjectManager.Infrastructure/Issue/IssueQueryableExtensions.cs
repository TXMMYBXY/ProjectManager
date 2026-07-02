using ProjectManager.Application.Issue;
using ProjectManager.Application.Issue.Dto;

namespace ProjectManager.Infrastructure.Issue;

public static class IssueQueryableExtensions
{
    public static IQueryable<Entities.Models.Issue> ApplyFilter(this IQueryable<Entities.Models.Issue> query,
        IssueFilter filter)
    {
        if(!string.IsNullOrWhiteSpace(filter.Title))
            query = query.Where(i => i.Title.Contains(filter.Title));
        
        if(filter.Status != null)
            query = query.Where(i => i.Status == filter.Status);
        
        if(filter.Priority != null)
            query = query.Where(i => i.Priority == filter.Priority);

        if (!string.IsNullOrWhiteSpace(filter.ProjectTitle))
            query = query.Where(i => i.Project.Title.Contains(filter.ProjectTitle));
        
        if(!string.IsNullOrWhiteSpace(filter.AuthorFullName))
            query = query.Where(i => i.Author.FirstName.Contains(filter.AuthorFullName));

        if (!string.IsNullOrWhiteSpace(filter.ExecutorFullName))
            query = query.Where(i => i.Executor.FirstName.Contains(filter.ExecutorFullName));
        
        return query;
    }

    public static IQueryable<Entities.Models.Issue> ApplySorting(this IQueryable<Entities.Models.Issue> query,
        IssueFilter filter)
    {
        if (filter.SortField.HasValue)
        {
            query = filter.SortField switch
            {
                IssueSortField.Title =>
                    filter.Descending
                        ? query.OrderBy(i => i.Title)
                        : query.OrderByDescending(i => i.Title),

                IssueSortField.Status =>
                    filter.Descending
                        ? query.OrderBy(i => i.Status)
                        : query.OrderByDescending(i => i.Status),

                IssueSortField.Priority =>
                    filter.Descending
                        ? query.OrderBy(i => i.Priority)
                        : query.OrderByDescending(i => i.Priority),

                IssueSortField.ProjectTitle =>
                    filter.Descending
                        ? query.OrderBy(i => i.Project.Title)
                        : query.OrderByDescending(i => i.Project.Title),

                IssueSortField.AuthorLastName =>
                    filter.Descending
                        ? query.OrderBy(i => i.Author.LastName)
                        : query.OrderByDescending(i => i.Author.LastName),

                IssueSortField.ExecutorLastName =>
                    filter.Descending
                        ? query.OrderBy(i => i.Executor.LastName)
                        : query.OrderByDescending(i => i.Executor.LastName),

                _ =>
                    filter.Descending
                        ? query.OrderBy(i => i.Priority)
                        : query.OrderByDescending(i => i.Priority)
            };
        }

        return query;
    }

    public static IQueryable<IssueItemDto> ApplyPagination(this IQueryable<IssueItemDto> query,
        IssueFilter filter)
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