using SellerMetrics.Application.Profit.DTOs;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.Profit.Queries;

/// <summary>
/// Query to get profit for a specific service job.
/// </summary>
public record GetServiceJobProfitQuery(int ServiceJobId, string Currency = "USD");

/// <summary>
/// Handler for GetServiceJobProfitQuery.
/// </summary>
public class GetServiceJobProfitQueryHandler
{
    private readonly IRevenueEntryRepository _revenueRepository;
    private readonly IComponentItemRepository _componentRepository;
    private readonly IRepository<Domain.Entities.ServiceJob> _serviceJobRepository;

    public GetServiceJobProfitQueryHandler(
        IRevenueEntryRepository revenueRepository,
        IComponentItemRepository componentRepository,
        IRepository<Domain.Entities.ServiceJob> serviceJobRepository)
    {
        _revenueRepository = revenueRepository;
        _componentRepository = componentRepository;
        _serviceJobRepository = serviceJobRepository;
    }

    public async Task<ServiceJobProfitDto?> HandleAsync(
        GetServiceJobProfitQuery query,
        CancellationToken cancellationToken = default)
    {
        var serviceJob = await _serviceJobRepository.GetByIdAsync(query.ServiceJobId, cancellationToken);
        if (serviceJob == null)
        {
            return null;
        }

        // Get revenue entries linked to this service job
        var revenueEntries = await _revenueRepository.GetByServiceJobIdAsync(query.ServiceJobId, cancellationToken);
        var filteredRevenue = revenueEntries
            .Where(r => r.GrossAmount.Currency == query.Currency)
            .ToList();

        var revenue = filteredRevenue.Sum(r => r.GrossAmount.Amount - r.Fees.Amount);

        // Get components used in this job
        var components = await _componentRepository.GetByServiceJobIdAsync(query.ServiceJobId, cancellationToken);
        var componentCosts = components
            .Where(c => c.Status == ComponentStatus.Used && c.UnitCost.Currency == query.Currency)
            .Sum(c => c.TotalValue.Amount);

        // Calculate profit (expenses will be added when BusinessExpense is linked)
        var expenses = 0m;
        var profit = revenue - componentCosts - expenses;
        var profitMargin = revenue > 0 ? (profit / revenue) * 100 : 0;

        return new ServiceJobProfitDto
        {
            ServiceJobId = serviceJob.Id,
            JobNumber = serviceJob.JobNumber,
            CustomerName = serviceJob.CustomerName,
            Revenue = revenue,
            ComponentCosts = componentCosts,
            Expenses = expenses,
            Profit = profit,
            ProfitMargin = profitMargin,
            Currency = query.Currency,
            RevenueFormatted = new Money(revenue, query.Currency).ToString(),
            ComponentCostsFormatted = new Money(componentCosts, query.Currency).ToString(),
            ExpensesFormatted = new Money(expenses, query.Currency).ToString(),
            ProfitFormatted = new Money(profit, query.Currency).ToString()
        };
    }
}
