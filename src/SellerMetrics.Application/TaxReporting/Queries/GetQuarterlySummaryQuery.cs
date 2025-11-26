using SellerMetrics.Application.Expenses.DTOs;
using SellerMetrics.Application.TaxReporting.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.TaxReporting.Queries;

/// <summary>
/// Query to get quarterly tax summary.
/// </summary>
public record GetQuarterlySummaryQuery(
    int Year,
    int Quarter,
    string Currency = "USD");

/// <summary>
/// Handler for GetQuarterlySummaryQuery.
/// </summary>
public class GetQuarterlySummaryQueryHandler
{
    private readonly IRevenueEntryRepository _revenueRepository;
    private readonly IBusinessExpenseRepository _expenseRepository;
    private readonly IMileageEntryRepository _mileageRepository;
    private readonly IIrsMileageRateRepository _mileageRateRepository;
    private readonly IEstimatedTaxPaymentRepository _taxPaymentRepository;

    public GetQuarterlySummaryQueryHandler(
        IRevenueEntryRepository revenueRepository,
        IBusinessExpenseRepository expenseRepository,
        IMileageEntryRepository mileageRepository,
        IIrsMileageRateRepository mileageRateRepository,
        IEstimatedTaxPaymentRepository taxPaymentRepository)
    {
        _revenueRepository = revenueRepository;
        _expenseRepository = expenseRepository;
        _mileageRepository = mileageRepository;
        _mileageRateRepository = mileageRateRepository;
        _taxPaymentRepository = taxPaymentRepository;
    }

    public async Task<QuarterlySummaryDto> HandleAsync(
        GetQuarterlySummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        // Get the date range for this quarter (calendar year for tax purposes)
        var (startDate, endDate) = GetQuarterDateRange(query.Year, query.Quarter);

        // Get revenue data
        var revenues = await _revenueRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        var filteredRevenues = revenues
            .Where(r => r.GrossAmount.Currency == query.Currency)
            .ToList();

        // Calculate revenue by source
        var revenueBySource = CalculateRevenueBySource(filteredRevenues, query.Currency);
        var ebayRevenue = revenueBySource.FirstOrDefault(r => r.Source == RevenueSource.eBay)?.GrossRevenue ?? 0;
        var serviceRevenue = revenueBySource.FirstOrDefault(r => r.Source == RevenueSource.ComputerServices)?.GrossRevenue ?? 0;
        var totalRevenue = ebayRevenue + serviceRevenue;

        // Get expense data
        var expenses = await _expenseRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        var filteredExpenses = expenses
            .Where(e => e.Amount.Currency == query.Currency && e.IsTaxDeductible)
            .ToList();

        var expensesByCategory = CalculateExpensesByCategory(filteredExpenses, query.Currency);
        var totalExpenses = filteredExpenses.Sum(e => e.Amount.Amount);

        // Get mileage data
        var mileageEntries = await _mileageRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        var mileageRate = await _mileageRateRepository.GetByDateAsync(endDate, cancellationToken);
        var mileageBreakdown = CalculateMileageDeduction(mileageEntries, mileageRate, query.Currency, totalRevenue, ebayRevenue, serviceRevenue);

        // Get estimated tax payment for this quarter
        var taxPayment = await _taxPaymentRepository.GetByQuarterAsync(query.Year, query.Quarter, cancellationToken);
        var taxPaymentDto = taxPayment != null ? MapTaxPaymentToDto(taxPayment) : null;

        // Calculate net profit
        var netProfit = totalRevenue - totalExpenses - mileageBreakdown.TotalDeduction;

        return new QuarterlySummaryDto
        {
            Year = query.Year,
            Quarter = query.Quarter,
            QuarterDisplay = $"Q{query.Quarter} {query.Year}",
            StartDate = startDate,
            EndDate = endDate,
            EbayRevenue = ebayRevenue,
            ServiceRevenue = serviceRevenue,
            TotalRevenue = totalRevenue,
            RevenueBySource = revenueBySource,
            TotalExpenses = totalExpenses,
            ExpensesByCategory = expensesByCategory,
            TotalMiles = mileageBreakdown.TotalMiles,
            MileageDeduction = mileageBreakdown.TotalDeduction,
            MileageBreakdown = mileageBreakdown,
            NetProfit = netProfit,
            EstimatedTaxPayment = taxPaymentDto,
            Currency = query.Currency,
            EbayRevenueFormatted = new Money(ebayRevenue, query.Currency).ToString(),
            ServiceRevenueFormatted = new Money(serviceRevenue, query.Currency).ToString(),
            TotalRevenueFormatted = new Money(totalRevenue, query.Currency).ToString(),
            TotalExpensesFormatted = new Money(totalExpenses, query.Currency).ToString(),
            MileageDeductionFormatted = new Money(mileageBreakdown.TotalDeduction, query.Currency).ToString(),
            NetProfitFormatted = new Money(netProfit, query.Currency).ToString()
        };
    }

    private static (DateTime Start, DateTime End) GetQuarterDateRange(int year, int quarter)
    {
        // Use standard calendar quarters for tax reporting
        return quarter switch
        {
            1 => (new DateTime(year, 1, 1), new DateTime(year, 3, 31)),
            2 => (new DateTime(year, 4, 1), new DateTime(year, 6, 30)),
            3 => (new DateTime(year, 7, 1), new DateTime(year, 9, 30)),
            4 => (new DateTime(year, 10, 1), new DateTime(year, 12, 31)),
            _ => throw new ArgumentOutOfRangeException(nameof(quarter), "Quarter must be 1-4")
        };
    }

    private static List<RevenueBySourceSummaryDto> CalculateRevenueBySource(
        List<RevenueEntry> revenues,
        string currency)
    {
        return revenues
            .GroupBy(r => r.Source)
            .Select(g => new RevenueBySourceSummaryDto
            {
                Source = g.Key,
                SourceDisplay = g.Key == RevenueSource.eBay ? "eBay" : "Computer Services",
                GrossRevenue = g.Sum(r => r.GrossAmount.Amount),
                Fees = g.Sum(r => r.Fees.Amount),
                NetRevenue = g.Sum(r => r.GrossAmount.Amount - r.Fees.Amount),
                TransactionCount = g.Count(),
                Currency = currency,
                GrossRevenueFormatted = new Money(g.Sum(r => r.GrossAmount.Amount), currency).ToString(),
                FeesFormatted = new Money(g.Sum(r => r.Fees.Amount), currency).ToString(),
                NetRevenueFormatted = new Money(g.Sum(r => r.GrossAmount.Amount - r.Fees.Amount), currency).ToString()
            })
            .OrderBy(r => r.Source)
            .ToList();
    }

    private static List<ExpenseByCategoryDto> CalculateExpensesByCategory(
        List<BusinessExpense> expenses,
        string currency)
    {
        return expenses
            .GroupBy(e => e.Category)
            .Select(g => new ExpenseByCategoryDto
            {
                Category = g.Key,
                CategoryDisplay = g.Key.GetDisplayName(),
                ScheduleCLine = g.Key.GetScheduleCLine(),
                ScheduleCLineLabel = g.Key.GetScheduleCLineLabel(),
                ScheduleCDescription = g.Key.GetScheduleCDescription(),
                Total = g.Sum(e => e.Amount.Amount),
                Currency = currency,
                TotalFormatted = new Money(g.Sum(e => e.Amount.Amount), currency).ToString(),
                ExpenseCount = g.Count()
            })
            .OrderBy(c => c.ScheduleCLine)
            .ToList();
    }

    private static MileageDeductionSummaryDto CalculateMileageDeduction(
        IReadOnlyList<MileageEntry> entries,
        IrsMileageRate? rate,
        string currency,
        decimal totalRevenue,
        decimal ebayRevenue,
        decimal serviceRevenue)
    {
        var applicableRate = rate?.StandardRate ?? 0.70m; // Default to 2025 rate

        var ebayMiles = entries
            .Where(e => e.BusinessLine == BusinessLine.eBay)
            .Sum(e => e.TotalMiles);

        var serviceMiles = entries
            .Where(e => e.BusinessLine == BusinessLine.ComputerServices)
            .Sum(e => e.TotalMiles);

        var sharedMiles = entries
            .Where(e => e.BusinessLine == BusinessLine.Shared)
            .Sum(e => e.TotalMiles);

        // Allocate shared miles proportionally based on revenue
        decimal ebayShareOfShared = 0;
        decimal serviceShareOfShared = 0;
        if (totalRevenue > 0 && sharedMiles > 0)
        {
            var ebayRatio = ebayRevenue / totalRevenue;
            var serviceRatio = serviceRevenue / totalRevenue;
            ebayShareOfShared = sharedMiles * ebayRatio;
            serviceShareOfShared = sharedMiles * serviceRatio;
        }

        var totalMiles = ebayMiles + serviceMiles + sharedMiles;
        var totalDeduction = totalMiles * applicableRate;
        var ebayDeduction = (ebayMiles + ebayShareOfShared) * applicableRate;
        var serviceDeduction = (serviceMiles + serviceShareOfShared) * applicableRate;
        var sharedDeduction = sharedMiles * applicableRate;

        return new MileageDeductionSummaryDto
        {
            TotalMiles = totalMiles,
            EbayMiles = ebayMiles,
            ServiceMiles = serviceMiles,
            SharedMiles = sharedMiles,
            ApplicableRate = applicableRate,
            ApplicableRateFormatted = $"${applicableRate:N2}/mile",
            TotalDeduction = totalDeduction,
            EbayDeduction = ebayDeduction,
            ServiceDeduction = serviceDeduction,
            SharedDeduction = sharedDeduction,
            TripCount = entries.Count,
            Currency = currency,
            TotalDeductionFormatted = new Money(totalDeduction, currency).ToString()
        };
    }

    private static EstimatedTaxPaymentDto MapTaxPaymentToDto(EstimatedTaxPayment payment)
    {
        return new EstimatedTaxPaymentDto
        {
            Id = payment.Id,
            TaxYear = payment.TaxYear,
            Quarter = payment.Quarter,
            QuarterDisplay = payment.QuarterDisplay,
            DueDate = payment.DueDate,
            DueDateFormatted = payment.DueDate.ToString("MMMM d, yyyy"),
            EstimatedAmount = payment.EstimatedAmount.Amount,
            EstimatedAmountFormatted = payment.EstimatedAmount.ToString(),
            AmountPaid = payment.AmountPaid.Amount,
            AmountPaidFormatted = payment.AmountPaid.ToString(),
            PaidDate = payment.PaidDate,
            IsPaid = payment.IsPaid,
            IsOverdue = payment.IsOverdue,
            RemainingAmount = payment.RemainingAmount,
            RemainingAmountFormatted = new Money(payment.RemainingAmount, payment.EstimatedAmount.Currency).ToString(),
            ConfirmationNumber = payment.ConfirmationNumber,
            PaymentMethod = payment.PaymentMethod,
            Notes = payment.Notes,
            Currency = payment.EstimatedAmount.Currency
        };
    }
}
