using System.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ProjectManager.Api;
using ProjectManager.Api.Middleware;
using ProjectManager.Infrastructure;
using ProjectManager.Infrastructure.Configuration;
using ProjectManager.Infrastructure.Data;
using Scalar.AspNetCore;
using ProjectManager.Api.Features.Account.Auth;
using Microsoft.AspNetCore.Identity;
using ProjectManager.Entities.Models;
using Microsoft.Data.SqlClient;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.Configure<DataBaseConnectionSettings>(builder.Configuration.GetSection(nameof(DataBaseConnectionSettings)));

builder.Services.AddOpenApi();

builder.Services.AddApi(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthorizationPolicy();

var app = builder.Build();

app.UseErrorHandling();

var httpsPort = builder.Configuration["ASPNETCORE_HTTPS_PORT"];
var urls = builder.Configuration["ASPNETCORE_URLS"];
if (!string.IsNullOrEmpty(httpsPort) || (urls != null && urls.Contains("https", StringComparison.OrdinalIgnoreCase)))
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

Task.Run(async () =>
{
    using var scope = app.Services.CreateScope();
    try
    {
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<ApplicationDbContext>();

        var maxAttempts = 30;
        var delayMs = 2000;
        var serverReady = false;

        var initialConnStr = db.Database.GetDbConnection().ConnectionString;
        var masterBuilder = new SqlConnectionStringBuilder(initialConnStr)
        {
            InitialCatalog = "master"
        };

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                using var masterConn = new SqlConnection(masterBuilder.ConnectionString);
                await masterConn.OpenAsync();
                serverReady = true;
                // ensure database exists
                var targetDb = new SqlConnectionStringBuilder(initialConnStr).InitialCatalog;
                using var cmd = masterConn.CreateCommand();
                cmd.CommandText = $"IF DB_ID(N'{targetDb}') IS NULL CREATE DATABASE [{targetDb}];";
                await cmd.ExecuteNonQueryAsync();
                break;
            }
            catch (Exception ex)
            {
                Log.Logger.Warning(ex, "SQL Server not ready yet (attempt {Attempt})", attempt);
            }

            await Task.Delay(delayMs);
        }

        if (!serverReady)
        {
            throw new DataException("Unable to connect to SQL Server after multiple attempts");
        }

        try
        {
            db.Database.Migrate();
        }
        catch (SqlException sqlEx) when (sqlEx.Number == 1801)
        {
            Log.Logger.Warning(sqlEx, "Database already exists (ignored) during creation");
        }

        var userManager = services.GetRequiredService<UserManager<Employee>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

        var roles = new[] { "Director", "Manager", "Employee" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int> { Name = role, NormalizedName = role.ToUpper() });
            }
        }

        var seeds = new Dictionary<string, string>
        {
            { "Director", "director@example.com" },
            { "Manager", "manager@example.com" },
            { "Employee", "employee@example.com" }
        };

        foreach (var kv in seeds)
        {
            var role = kv.Key;
            var email = kv.Value;
            var existing = await userManager.FindByEmailAsync(email);
            if (existing != null) continue;

            var user = new Employee
            {
                UserName = email,
                Email = email,
                FirstName = role,
                LastName = "Seed",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "1234");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
            else
            {
                Log.Logger.Warning("Failed to create seed user {Email}: {Errors}", email, string.Join(';', result.Errors.Select(e => e.Description)));
            }
        }
    }
    catch (Exception ex)
    {
        if (ex is InvalidOperationException)
        {
            Log.Logger.Error(ex, "Migration error (possible pending model changes). Add a new migration if necessary.");
        }
        else
        {
            Log.Logger.Error(ex, "Error while migrating or seeding database");
            throw;
        }
    }
}).GetAwaiter().GetResult();

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}

app.MapGet("/ping", () => "pong");