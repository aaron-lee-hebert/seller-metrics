using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure PostgreSQL database context
        services.AddDbContext<SellerMetricsDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(SellerMetricsDbContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                }));

        // Configure ASP.NET Core Identity
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

                // Sign-in settings
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
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
        services.AddScoped<Application.Inventory.Commands.MarkInventoryItemAsSoldCommandHandler>();
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
        services.AddScoped<Application.TaxReporting.Queries.GetQuarterlySummaryQueryHandler>();

        return services;
    }
}
