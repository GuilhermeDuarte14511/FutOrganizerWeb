using FutOrganizerWeb.Hubs;
using FutOrganizerWeb.Infrastructure.Config;
using FutOrganizerWeb.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FutOrganizerDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );
        }));

// Injeção de dependência dos serviços do projeto
builder.Services.AddProjectServices();

// Serviços MVC e sessão
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// Adiciona suporte ao SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Tratamento de erros e HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapHub<LobbyChatHub>("/hubs/lobbychat");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Logar}/{id?}");

app.Run();
