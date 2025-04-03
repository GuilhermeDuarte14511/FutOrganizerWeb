using FutOrganizerWeb.Infrastructure.Config;
using FutOrganizerWeb.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FutOrganizerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddProjectServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs");
}

app.MapGet("/", context =>
{
    context.Response.Redirect("/docs");
    return Task.CompletedTask;
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
