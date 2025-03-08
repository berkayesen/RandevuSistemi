using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RandevuSistemi.Data;
using RandevuSistemi.Models;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// MSSQL veritabanı bağlantısını al
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity Kullanıcı Yönetimi
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Admin kullanıcı oluşturma fonksiyonunu başlatmadan önce uygulamanın yapılandırması tamamlanmalı
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await CreateAdminUserAsync(services);  // Admin kullanıcı oluşturma fonksiyonu
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static async Task CreateAdminUserAsync(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string adminRole = "Admin";
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    // Admin kullanıcı bilgileri
    string adminEmail = "admin@example.com";
    string adminPassword = "Admin123!";
    string adminNick = "admin";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminNick,
            Email = adminEmail,
            FullName = "Admin Kullanıcı"
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
            Console.WriteLine("✅ Admin kullanıcı başarıyla oluşturuldu.");
        }
        else
        {
            Console.WriteLine("❌ Admin oluşturulurken hata oluştu.");
        }
    }
}
