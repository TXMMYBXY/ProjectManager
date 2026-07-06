using Microsoft.EntityFrameworkCore;
using Serilog;
using ProjectManager.Api;
using ProjectManager.Api.Middleware;
using ProjectManager.Infrastructure;
using ProjectManager.Infrastructure.Configuration;
using ProjectManager.Infrastructure.Data;
using Scalar.AspNetCore;
using ProjectManager.Api.Features.Account.Auth;


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
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     
//     db.Database.Migrate(); 
// }

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}

app.MapGet("/ping", () => "pong");