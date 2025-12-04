using AspNetCoreRateLimit;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using SellerMetrics.Infrastructure;
using SellerMetrics.Infrastructure.Persistence;
using SellerMetrics.Infrastructure.Services;
using SellerMetrics.Web.Filters;
using SellerMetrics.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

// Configure Hangfire with SQL Server storage
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString));

builder.Services.AddHangfireServer();
builder.Services.AddScoped<EbayOrderSyncJob>();

// Configure rate limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.RealIpHeader = "X-Forwarded-For";
    options.GeneralRules = new List<RateLimitRule>
    {
        // General rate limit: 100 requests per minute
        new RateLimitRule
        {
            Endpoint = "*",
            Period = "1m",
            Limit = 100
        },
        // Login endpoint: 5 requests per 15 minutes
        new RateLimitRule
        {
            Endpoint = "*:/Identity/Account/Login*",
            Period = "15m",
            Limit = 5
        },
        // Register endpoint: 3 requests per 15 minutes
        new RateLimitRule
        {
            Endpoint = "*:/Identity/Account/Register*",
            Period = "15m",
            Limit = 3
        }
    };
});
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();

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

// Apply pending database migrations on startup
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();

    var retryCount = 0;
    const int maxRetries = 15;
    const int retryDelaySeconds = 5;

    while (retryCount < maxRetries)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SellerMetricsDbContext>();

            logger.LogInformation("Checking database connection and applying migrations (attempt {Attempt}/{MaxRetries})...", retryCount + 1, maxRetries);

            // Migrate() will create the database if it doesn't exist and apply all migrations.
            db.Database.Migrate();

            logger.LogInformation("Database migrations applied successfully.");
            break;
        }
        catch (Exception ex)
        {
            retryCount++;
            if (retryCount >= maxRetries)
            {
                logger.LogCritical(ex, "Failed to apply database migrations after {MaxRetries} attempts.", maxRetries);
                throw;
            }

            var errorMessage = ex is Microsoft.Data.SqlClient.SqlException sqlEx
                ? $"SQL Error {sqlEx.Number}: {sqlEx.Message}"
                : ex.Message;

            logger.LogWarning("Database migration attempt {RetryCount}/{MaxRetries} failed: {Error}. Retrying in {Delay} seconds...",
                retryCount, maxRetries, errorMessage, retryDelaySeconds);

            Thread.Sleep(TimeSpan.FromSeconds(retryDelaySeconds));
        }
    }
}

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

// Configure Hangfire dashboard (admin only)
app.MapHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

// Register recurring jobs for eBay sync
RecurringJob.AddOrUpdate<EbayOrderSyncJob>(
    "ebay-token-refresh",
    job => job.RefreshExpiredTokensAsync(),
    "*/5 * * * *"); // Every 5 minutes

RecurringJob.AddOrUpdate<EbayOrderSyncJob>(
    "ebay-order-sync",
    job => job.ExecuteAsync(),
    "0,15,30,45 * * * *"); // Every 15 minutes at :00, :15, :30, :45

app.Run();
