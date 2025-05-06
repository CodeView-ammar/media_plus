using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add localization support
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Get connection string and configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<MediaPlusDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add scoped services
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddHttpContextAccessor(); // ÇáÃÝÖá ÇÓÊÎÏÇã åÐÇ ÈÏáÇð ãä AddTransient

// Configure session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(3);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add MVC with localization
builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

// Supported cultures
var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("ar-SA")
};

// Configure localization options
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("ar-SA");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Configure custom port
builder.WebHost.UseUrls("http://*:5852");

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Dashboard/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Apply localization
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ar-SA"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures,
    ApplyCurrentCultureToResponseHeaders = true
});

// Enable session
app.UseSession();

// Handle HTTP status codes
app.UseStatusCodePagesWithReExecute("/Error/{0}");

// Map routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
