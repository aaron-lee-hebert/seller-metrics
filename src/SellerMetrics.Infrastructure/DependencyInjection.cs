using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using SellerMetrics.Application.Ebay.Interfaces;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Infrastructure.Persistence;
using SellerMetrics.Infrastructure.Persistence.Repositories;
using SellerMetrics.Infrastructure.Services;

namespace SellerMetrics.Infrastructure;

/// <summary>
/// Extension methods for configuring Infrastructure layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="environment">The hosting environment (optional, for environment-specific configuration).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment? environment = null)
    {
        // Configure SQL Server database context
        services.AddDbContext<SellerMetricsDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptions =>
                {
                    sqlServerOptions.MigrationsAssembly(typeof(SellerMetricsDbContext).Assembly.FullName);
                    sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
        });

        // Configure ASP.NET Core Identity
        var isDevelopment = environment?.IsDevelopment() ?? false;

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password requirements
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;

                // Sign-in settings - disable email confirmation in development
                options.SignIn.RequireConfirmedEmail = !isDevelopment;
                options.SignIn.RequireConfirmedAccount = !isDevelopment;
            })
            .AddEntityFrameworkStores<SellerMetricsDbContext>()
            .AddDefaultTokenProviders();

        // Configure SendGrid email service
        services.Configure<SendGridOptions>(configuration.GetSection(SendGridOptions.SectionName));
        services.AddTransient<IEmailSender, SendGridEmailSender>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register generic repository
        services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));

        // Register specialized repositories
        services.AddScoped<IStorageLocationRepository, StorageLocationRepository>();
        services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
        services.AddScoped<IComponentTypeRepository, ComponentTypeRepository>();
        services.AddScoped<IComponentItemRepository, ComponentItemRepository>();
        services.AddScoped<IRevenueEntryRepository, RevenueEntryRepository>();
        services.AddScoped<IFiscalYearConfigurationRepository, FiscalYearConfigurationRepository>();
        services.AddScoped<IBusinessExpenseRepository, BusinessExpenseRepository>();
        services.AddScoped<IMileageEntryRepository, MileageEntryRepository>();
        services.AddScoped<IIrsMileageRateRepository, IrsMileageRateRepository>();
        services.AddScoped<IEstimatedTaxPaymentRepository, EstimatedTaxPaymentRepository>();

        // Register Application layer handlers - Storage Locations
        services.AddScoped<Application.StorageLocations.Commands.CreateStorageLocationCommandHandler>();
        services.AddScoped<Application.StorageLocations.Commands.UpdateStorageLocationCommandHandler>();
        services.AddScoped<Application.StorageLocations.Commands.DeleteStorageLocationCommandHandler>();
        services.AddScoped<Application.StorageLocations.Queries.GetStorageLocationHierarchyQueryHandler>();
        services.AddScoped<Application.StorageLocations.Queries.GetStorageLocationQueryHandler>();
        services.AddScoped<Application.StorageLocations.Queries.GetAllStorageLocationsQueryHandler>();

        // Register Application layer handlers - Inventory
        services.AddScoped<Application.Inventory.Commands.CreateInventoryItemCommandHandler>();
        services.AddScoped<Application.Inventory.Commands.UpdateInventoryItemCommandHandler>();
        services.AddScoped<Application.Inventory.Commands.MoveInventoryItemCommandHandler>();
        services.AddScoped<Application.Inventory.Commands.SellInventoryItemCommandHandler>();
        services.AddScoped<Application.Inventory.Commands.DeleteInventoryItemCommandHandler>();
        services.AddScoped<Application.Inventory.Queries.GetInventoryListQueryHandler>();
        services.AddScoped<Application.Inventory.Queries.GetInventoryItemQueryHandler>();
        services.AddScoped<Application.Inventory.Queries.SearchInventoryQueryHandler>();
        services.AddScoped<Application.Inventory.Queries.GetInventoryValueQueryHandler>();

        // Register Application layer handlers - Components
        services.AddScoped<Application.Components.Commands.CreateComponentItemCommandHandler>();
        services.AddScoped<Application.Components.Commands.UpdateComponentItemCommandHandler>();
        services.AddScoped<Application.Components.Commands.AdjustComponentQuantityCommandHandler>();
        services.AddScoped<Application.Components.Commands.MoveComponentCommandHandler>();
        services.AddScoped<Application.Components.Commands.UseComponentCommandHandler>();
        services.AddScoped<Application.Components.Commands.DeleteComponentItemCommandHandler>();
        services.AddScoped<Application.Components.Queries.GetComponentListQueryHandler>();
        services.AddScoped<Application.Components.Queries.GetLowStockComponentsQueryHandler>();
        services.AddScoped<Application.Components.Queries.GetComponentValueQueryHandler>();
        services.AddScoped<Application.Components.Queries.GetComponentTypesQueryHandler>();

        // Register Application layer handlers - Revenue
        services.AddScoped<Application.Revenue.Commands.CreateRevenueEntryCommandHandler>();
        services.AddScoped<Application.Revenue.Commands.UpdateRevenueEntryCommandHandler>();
        services.AddScoped<Application.Revenue.Commands.DeleteRevenueEntryCommandHandler>();
        services.AddScoped<Application.Revenue.Queries.GetRevenueEntryQueryHandler>();
        services.AddScoped<Application.Revenue.Queries.GetRevenueListQueryHandler>();
        services.AddScoped<Application.Revenue.Queries.GetRevenueBySourceQueryHandler>();
        services.AddScoped<Application.Revenue.Queries.GetMonthlyRevenueQueryHandler>();
        services.AddScoped<Application.Revenue.Queries.GetQuarterlyRevenueQueryHandler>();
        services.AddScoped<Application.Revenue.Queries.GetYearToDateRevenueQueryHandler>();

        // Register Application layer handlers - Profit
        services.AddScoped<Application.Profit.Queries.GetProfitBySourceQueryHandler>();
        services.AddScoped<Application.Profit.Queries.GetCombinedProfitQueryHandler>();
        services.AddScoped<Application.Profit.Queries.GetQuarterlyProfitQueryHandler>();
        services.AddScoped<Application.Profit.Queries.GetServiceJobProfitQueryHandler>();
        services.AddScoped<Application.Profit.Queries.GetTaxReportProfitQueryHandler>();

        // Register Application layer handlers - Expenses
        services.AddScoped<Application.Expenses.Commands.CreateExpenseCommandHandler>();
        services.AddScoped<Application.Expenses.Commands.UpdateExpenseCommandHandler>();
        services.AddScoped<Application.Expenses.Commands.DeleteExpenseCommandHandler>();
        services.AddScoped<Application.Expenses.Queries.GetExpenseQueryHandler>();
        services.AddScoped<Application.Expenses.Queries.GetExpensesByCategoryQueryHandler>();
        services.AddScoped<Application.Expenses.Queries.GetExpensesByBusinessLineQueryHandler>();
        services.AddScoped<Application.Expenses.Queries.GetExpensesByDateRangeQueryHandler>();
        services.AddScoped<Application.Expenses.Queries.GetExpenseSummaryQueryHandler>();

        // Register Application layer handlers - Mileage
        services.AddScoped<Application.Mileage.Commands.CreateMileageEntryCommandHandler>();
        services.AddScoped<Application.Mileage.Commands.UpdateMileageEntryCommandHandler>();
        services.AddScoped<Application.Mileage.Commands.DeleteMileageEntryCommandHandler>();
        services.AddScoped<Application.Mileage.Queries.GetMileageEntryQueryHandler>();
        services.AddScoped<Application.Mileage.Queries.GetMileageLogQueryHandler>();
        services.AddScoped<Application.Mileage.Queries.CalculateMileageDeductionQueryHandler>();
        services.AddScoped<Application.Mileage.Queries.GetIrsMileageRatesQueryHandler>();

        // Register Application layer handlers - Tax Reporting
        services.AddScoped<Application.TaxReporting.Commands.CreateEstimatedTaxPaymentCommandHandler>();
        services.AddScoped<Application.TaxReporting.Commands.RecordTaxPaymentCommandHandler>();
        services.AddScoped<Application.TaxReporting.Commands.ExportTaxReportCommandHandler>();
        services.AddScoped<Application.TaxReporting.Queries.GetQuarterlySummaryQueryHandler>();
        services.AddScoped<Application.TaxReporting.Queries.GetAnnualSummaryQueryHandler>();

        // Register eBay repositories
        services.AddScoped<IEbayUserCredentialRepository, EbayUserCredentialRepository>();
        services.AddScoped<IEbayOrderRepository, EbayOrderRepository>();

        // Configure Data Protection for token encryption
        services.AddDataProtection()
            .SetApplicationName("SellerMetrics")
            .PersistKeysToFileSystem(new DirectoryInfo(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "SellerMetrics", "DataProtection-Keys")));

        // Register Token Encryption Service
        services.AddScoped<ITokenEncryptionService, TokenEncryptionService>();

        // Configure eBay API options
        services.Configure<EbayApiOptions>(configuration.GetSection(EbayApiOptions.SectionName));

        // Configure HttpClient for eBay API with retry policy
        services.AddHttpClient<IEbayApiClient, EbayApiClient>()
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

        // Register Application layer handlers - eBay
        services.AddScoped<Application.Ebay.Commands.ConnectEbayAccountCommandHandler>();
        services.AddScoped<Application.Ebay.Commands.DisconnectEbayAccountCommandHandler>();
        services.AddScoped<Application.Ebay.Commands.SyncOrdersFromEbayCommandHandler>();
        services.AddScoped<Application.Ebay.Commands.LinkOrderToInventoryCommandHandler>();
        services.AddScoped<Application.Ebay.Commands.UpdateShippingCostCommandHandler>();
        services.AddScoped<Application.Ebay.Queries.GetEbayConnectionStatusQueryHandler>();
        services.AddScoped<Application.Ebay.Queries.GetEbayAuthorizationUrlQueryHandler>();
        services.AddScoped<Application.Ebay.Queries.GetEbayOrderListQueryHandler>();
        services.AddScoped<Application.Ebay.Queries.GetEbayOrderQueryHandler>();
        services.AddScoped<Application.Ebay.Queries.GetEbayOrderStatsQueryHandler>();

        return services;
    }

    /// <summary>
    /// Gets the retry policy for HTTP requests.
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    /// <summary>
    /// Gets the circuit breaker policy for HTTP requests.
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }
}
