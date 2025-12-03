using AspNetCoreRateLimit;
using SellerMetrics.Infrastructure;
using SellerMetrics.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

// Configure rate limiting (only in production)
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddMemoryCache();
    builder.Services.Configure<IpRateLimitOptions>(options =>
    {
        options.EnableEndpointRateLimiting = true;
        options.StackBlockedRequests = false;
        options.HttpStatusCode = 429;
        options.RealIpHeader = "X-Forwarded-For";
        options.GeneralRules = new List<RateLimitRule>
        {
            // General rate limit: 1000 requests per minute (relaxed for development testing)
            new RateLimitRule
            {
                Endpoint = "*",
                Period = "1m",
                Limit = 1000
            },
            // Login endpoint: 20 requests per 15 minutes (relaxed)
            new RateLimitRule
            {
                Endpoint = "*:/Identity/Account/Login*",
                Period = "15m",
                Limit = 20
            },
            // Register endpoint: 10 requests per 15 minutes (relaxed)
            new RateLimitRule
            {
                Endpoint = "*:/Identity/Account/Register*",
                Period = "15m",
                Limit = 10
            }
        };
    });
    builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
    builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
    builder.Services.AddInMemoryRateLimiting();
}

// Configure cookie settings for authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    // Use SameAsRequest in development, Always in production
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Configure antiforgery (CSRF protection)
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    // Use SameAsRequest in development, Always in production
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Security headers middleware (apply to all responses)
app.UseSecurityHeaders();

// Rate limiting middleware (only in production)
if (!app.Environment.IsDevelopment())
{
    app.UseIpRateLimiting();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS is typically handled by reverse proxy, but included for direct access scenarios
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Authentication & Authorization middleware (order matters!)
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Map Razor Pages for Identity UI
app.MapRazorPages();

app.Run();
