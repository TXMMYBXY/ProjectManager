using Microsoft.EntityFrameworkCore;
using ProjectManager.Api;
using ProjectManager.Api.Middleware;
using ProjectManager.Infrastructure;
using ProjectManager.Infrastructure.Configuration;
using ProjectManager.Infrastructure.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DataBaseConnectionSettings>(builder.Configuration.GetSection(nameof(DataBaseConnectionSettings)));

builder.Services.AddOpenApi();

builder.Services.AddApi(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseErrorHandling();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
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

app.Run();

app.MapGet("/ping", () => "pong");