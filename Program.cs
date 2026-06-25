using UCCD_App.Extensions;
using UCCD_App.Context;
using UCCD_App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. استخدام الملفات الجديدة اللي عملناها (Extension Methods)
builder.Services.AddControllers();

builder.Services.AddApplicationServices(builder.Configuration); // بتنادي على ملف الـ App Services
builder.Services.AddIdentityServices(builder.Configuration);    // بتنادي على ملف الـ Identity
builder.Services.AddSignalR(); // Add SignalR

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// 2. الـ Middleware (الترتيب هنا مهم)
app.UseHttpsRedirection();
app.UseStaticFiles(); // Added for CV file uploads
app.UseRouting();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireCors("AllowAngular");
app.MapHub<UCCD_App.Hubs.NotificationHub>("/hubs/notifications").RequireCors("AllowAngular");

// 3. الـ Seed Data (خليناها برضه منظمة)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await SeedData.RoleSeed(roleManager);
        await SeedData.AdminSeed(userManager);
        await SeedData.MockDataSeed(context, userManager, roleManager, app.Environment.ContentRootPath);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error during seeding: " + ex.Message);
    }
}

app.Run();