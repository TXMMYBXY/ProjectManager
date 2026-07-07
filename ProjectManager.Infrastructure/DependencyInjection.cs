using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using ProjectManager.Application.Account;
using ProjectManager.Application.Common;
using ProjectManager.Application.Document;
using ProjectManager.Application.Employee;
using ProjectManager.Application.Issue;
using ProjectManager.Application.Project;
using ProjectManager.Infrastructure.Account;
using ProjectManager.Infrastructure.Common;
using ProjectManager.Infrastructure.Common.MappingProfile;
using ProjectManager.Infrastructure.Configuration;
using ProjectManager.Infrastructure.Data;
using ProjectManager.Infrastructure.Document;
using ProjectManager.Infrastructure.Employee;
using ProjectManager.Infrastructure.Issue;
using ProjectManager.Infrastructure.Project;

namespace ProjectManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        
        services.AddAutoMapper(typeof(ProjectMappingProfile).Assembly);
        services.AddAutoMapper(typeof(EmployeeMappingProfile).Assembly);
        services.AddAutoMapper(typeof(IssueMappingProfile).Assembly);
        services.AddAutoMapper(typeof(AccountMappingProfile).Assembly);
        
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IIssueService, IssueService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IDocumentService, DocumentService>();
        
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IIssueRepository, IssueRepository>();
        services.AddScoped<IEmployeeProjectRepository, EmployeeProjectRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();

        
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

            // Игнорируем предупреждение о несинхронизированных миграциях при запуске автоприменения миграций.
            options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });
        
        services.AddHttpContextAccessor();
        
        services.AddIdentity<Entities.Models.Employee, IdentityRole<int>>(options =>
        {
            options.Password.RequiredLength = 4;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireDigit = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager();
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =
            options.DefaultChallengeScheme =
            options.DefaultForbidScheme =
            options.DefaultScheme =
            options.DefaultSignInScheme =
            options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["JwtSettings:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"])
                ),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role
            };
        });
        
        return services;
    }
}