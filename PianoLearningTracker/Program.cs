using Microsoft.EntityFrameworkCore;
using PianoLearningTracker.DAL;
using PianoLearningTracker.Models;
using PianoLearningTracker.Repositories;
using PianoLearningTracker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IMockRepository, EfRepository>();

builder.Services.AddDbContext<PianoLearningTrackerDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PianoLearningTrackerDbContext")));

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
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
