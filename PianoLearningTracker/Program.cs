using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PianoLearningTracker.DAL;
using PianoLearningTracker.Models;
using PianoLearningTracker.Repositories;
using PianoLearningTracker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddScoped<IMockRepository, EfRepository>();

builder.Services.AddDbContext<PianoLearningTrackerDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PianoLearningTrackerDbContext")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<PianoLearningTrackerDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        options.CallbackPath = "/signin-google";
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IAiLoggerService, AiLoggerService>();
builder.Services.AddScoped<IAnthropicService, AnthropicService>();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Seed admin korisnika pri pokretanju
using (var scope = app.Services.CreateScope())
{
    // EnsureCreated() applies HasData seeds for InMemory (test) provider; is a no-op on existing SQL Server DB
    var dbContext = scope.ServiceProvider.GetRequiredService<PianoLearningTrackerDbContext>();
    dbContext.Database.EnsureCreated();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    const string adminEmail = "admin@glazbena.hr";

    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(admin, "Admin123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, "Administrator");
    }
}

app.Run();

public partial class Program { }
