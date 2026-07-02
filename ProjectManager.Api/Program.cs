using Microsoft.EntityFrameworkCore;
using ProjectManager.Infrastructure;
using ProjectManager.Infrastructure.Configuration;
using ProjectManager.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DataBaseConnectionSettings>(builder.Configuration.GetSection(nameof(DataBaseConnectionSettings)));

builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate(); 
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

app.MapGet("/ping", () => "pong");