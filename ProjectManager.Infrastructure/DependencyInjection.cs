using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManager.Application.Common;
using ProjectManager.Application.Employee;
using ProjectManager.Application.Project;
using ProjectManager.Infrastructure.Common;
using ProjectManager.Infrastructure.Configuration;
using ProjectManager.Infrastructure.Data;
using ProjectManager.Infrastructure.Project;

namespace ProjectManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IProjectRepository, ProjectRepository>();
        
        var dataBaseConnectionSettings =
            configuration.GetSection("DataBaseConnectionSettings").Get<DataBaseConnectionSettings>();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            Console.WriteLine(dataBaseConnectionSettings.ConnectionString);
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