using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManager.Application.Common;
using ProjectManager.Application.Employee;
using ProjectManager.Application.Issue;
using ProjectManager.Application.Project;
using ProjectManager.Infrastructure.Common;
using ProjectManager.Infrastructure.Common.MappingProfile;
using ProjectManager.Infrastructure.Configuration;
using ProjectManager.Infrastructure.Data;
using ProjectManager.Infrastructure.Employee;
using ProjectManager.Infrastructure.Issue;
using ProjectManager.Infrastructure.Project;

namespace ProjectManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(ProjectMappingProfile).Assembly);
        services.AddAutoMapper(typeof(EmployeeMappingProfile).Assembly);
        services.AddAutoMapper(typeof(IssueMappingProfile).Assembly);
        
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IIssueService, IssueService>();
        
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IIssueRepository, IssueRepository>();
        
        var dataBaseConnectionSettings =
            configuration.GetSection("DataBaseConnectionSettings").Get<DataBaseConnectionSettings>();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(dataBaseConnectionSettings.ConnectionString,
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });
        });
        
        return services;
    }
}