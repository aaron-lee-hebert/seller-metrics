using SellerMetrics.Application.Expenses.DTOs;
using SellerMetrics.Application.TaxReporting.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;
using SellerMetrics.Domain.Interfaces;
using SellerMetrics.Domain.ValueObjects;

namespace SellerMetrics.Application.TaxReporting.Queries;

/// <summary>
/// Query to get annual tax summary (calendar year).
/// </summary>
public record GetAnnualSummaryQuery(
    int Year,
    string Currency = "USD");

/// <summary>
/// Handler for GetAnnualSummaryQuery.
/// </summary>
public class GetAnnualSummaryQueryHandler
{
    private readonly IRevenueEntryRepository _revenueRepository;
    private readonly IBusinessExpenseRepository _expenseRepository;
    private readonly IMileageEntryRepository _mileageRepository;
    private readonly IIrsMileageRateRepository _mileageRateRepository;
    private readonly IEstimatedTaxPaymentRepository _taxPaymentRepository;
    private readonly IInventoryItemRepository _inventoryRepository;

    public GetAnnualSummaryQueryHandler(
        IRevenueEntryRepository revenueRepository,
        IBusinessExpenseRepository expenseRepository,
        IMileageEntryRepository mileageRepository,
        IIrsMileageRateRepository mileageRateRepository,
        IEstimatedTaxPaymentRepository taxPaymentRepository,
        IInventoryItemRepository inventoryRepository)
    {
        _revenueRepository = revenueRepository;
        _expenseRepository = expenseRepository;
        _mileageRepository = mileageRepository;
        _mileageRateRepository = mileageRateRepository;
        _taxPaymentRepository = taxPaymentRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<AnnualSummaryDto> HandleAsync(
        GetAnnualSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        var startDate = new DateTime(query.Year, 1, 1);
        var endDate = new DateTime(query.Year, 12, 31);

        // Get all quarterly summaries
        var quarterlyBreakdown = new List<QuarterlySummaryDto>();
        for (int q = 1; q <= 4; q++)
        {
            var quarterSummary = await GetQuarterSummaryAsync(query.Year, q, query.Currency, cancellationToken);
            quarterlyBreakdown.Add(quarterSummary);
        }

        // Aggregate annual totals from quarterly data
        var totalGrossRevenue = quarterlyBreakdown.Sum(q => q.TotalRevenue);
        var ebayRevenue = quarterlyBreakdown.Sum(q => q.EbayRevenue);
        var serviceRevenue = quarterlyBreakdown.Sum(q => q.ServiceRevenue);
        var totalExpenses = quarterlyBreakdown.Sum(q => q.TotalExpenses);
        var totalMileageDeduction = quarterlyBreakdown.Sum(q => q.MileageDeduction);
        var netProfit = quarterlyBreakdown.Sum(q => q.NetProfit);

        // Calculate fees and shipping from revenue entries (Schedule C deductions)
        var revenues = await _revenueRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        var totalFees = revenues
            .Where(r => r.Fees.Currency == query.Currency)
            .Sum(r => r.Fees.Amount);
        var totalShipping = revenues
            .Where(r => r.Shipping.Currency == query.Currency)
            .Sum(r => r.Shipping.Amount);
        var totalNetRevenue = totalGrossRevenue - totalFees - totalShipping;

        // Get estimated tax payments
        var taxPayments = await _taxPaymentRepository.GetByTaxYearAsync(query.Year, cancellationToken);
        var taxPaymentDtos = taxPayments.Select(MapTaxPaymentToDto).ToList();

        // Build Schedule C summary
        var scheduleC = await BuildScheduleCSummaryAsync(
            query.Year,
            totalGrossRevenue,
            totalFees,
            totalShipping,
            totalExpenses,
            totalMileageDeduction,
            netProfit,
            query.Currency,
            cancellationToken);

        return new AnnualSummaryDto
        {
            Year = query.Year,
            TotalGrossRevenue = totalGrossRevenue,
            TotalFees = totalFees,
            TotalShipping = totalShipping,
            TotalNetRevenue = totalNetRevenue,
            EbayRevenue = ebayRevenue,
            ServiceRevenue = serviceRevenue,
            TotalExpenses = totalExpenses,
            TotalMileageDeduction = totalMileageDeduction,
            NetProfit = netProfit,
            QuarterlyBreakdown = quarterlyBreakdown,
            ScheduleC = scheduleC,
            EstimatedTaxPayments = taxPaymentDtos,
            Currency = query.Currency,
            TotalGrossRevenueFormatted = new Money(totalGrossRevenue, query.Currency).ToString(),
            TotalFeesFormatted = new Money(totalFees, query.Currency).ToString(),
            TotalShippingFormatted = new Money(totalShipping, query.Currency).ToString(),
            TotalNetRevenueFormatted = new Money(totalNetRevenue, query.Currency).ToString(),
            EbayRevenueFormatted = new Money(ebayRevenue, query.Currency).ToString(),
            ServiceRevenueFormatted = new Money(serviceRevenue, query.Currency).ToString(),
            TotalExpensesFormatted = new Money(totalExpenses, query.Currency).ToString(),
            TotalMileageDeductionFormatted = new Money(totalMileageDeduction, query.Currency).ToString(),
            NetProfitFormatted = new Money(netProfit, query.Currency).ToString()
        };
    }

    private async Task<QuarterlySummaryDto> GetQuarterSummaryAsync(
        int year,
        int quarter,
        string currency,
        CancellationToken cancellationToken)
    {
        var (startDate, endDate) = GetQuarterDateRange(year, quarter);

        // Get revenue data
        var revenues = await _revenueRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        var filteredRevenues = revenues
            .Where(r => r.GrossAmount.Currency == currency)
            .ToList();

        var ebayRevenue = filteredRevenues
            .Where(r => r.Source == RevenueSource.eBay)
            .Sum(r => r.GrossAmount.Amount);
        var serviceRevenue = filteredRevenues
            .Where(r => r.Source == RevenueSource.ComputerServices)
            .Sum(r => r.GrossAmount.Amount);
        var totalRevenue = ebayRevenue + serviceRevenue;

        // Calculate fees and shipping (Schedule C deductions)
        var totalFees = filteredRevenues.Sum(r => r.Fees.Amount);
        var totalShipping = filteredRevenues.Sum(r => r.Shipping.Amount);

        // Get expense data
        var expenses = await _expenseRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        var filteredExpenses = expenses
            .Where(e => e.Amount.Currency == currency && e.IsTaxDeductible)
            .ToList();
        var totalExpenses = filteredExpenses.Sum(e => e.Amount.Amount);

        var expensesByCategory = filteredExpenses
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

        // Get mileage data
        var mileageEntries = await _mileageRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        var mileageRate = await _mileageRateRepository.GetByDateAsync(endDate, cancellationToken);
        var applicableRate = mileageRate?.StandardRate ?? 0.70m;

        var totalMiles = mileageEntries.Sum(m => m.TotalMiles);
        var mileageDeduction = totalMiles * applicableRate;

        // Get tax payment
        var taxPayment = await _taxPaymentRepository.GetByQuarterAsync(year, quarter, cancellationToken);

        // Calculate net profit (Revenue - Fees - Shipping - Expenses - Mileage Deduction)
        var netProfit = totalRevenue - totalFees - totalShipping - totalExpenses - mileageDeduction;

        return new QuarterlySummaryDto
        {
            Year = year,
            Quarter = quarter,
            QuarterDisplay = $"Q{quarter} {year}",
            StartDate = startDate,
            EndDate = endDate,
            EbayRevenue = ebayRevenue,
            ServiceRevenue = serviceRevenue,
            TotalRevenue = totalRevenue,
            TotalFees = totalFees,
            TotalShipping = totalShipping,
            TotalExpenses = totalExpenses,
            ExpensesByCategory = expensesByCategory,
            TotalMiles = totalMiles,
            MileageDeduction = mileageDeduction,
            NetProfit = netProfit,
            EstimatedTaxPayment = taxPayment != null ? MapTaxPaymentToDto(taxPayment) : null,
            Currency = currency,
            EbayRevenueFormatted = new Money(ebayRevenue, currency).ToString(),
            ServiceRevenueFormatted = new Money(serviceRevenue, currency).ToString(),
            TotalRevenueFormatted = new Money(totalRevenue, currency).ToString(),
            TotalFeesFormatted = new Money(totalFees, currency).ToString(),
            TotalShippingFormatted = new Money(totalShipping, currency).ToString(),
            TotalExpensesFormatted = new Money(totalExpenses, currency).ToString(),
            MileageDeductionFormatted = new Money(mileageDeduction, currency).ToString(),
            NetProfitFormatted = new Money(netProfit, currency).ToString()
        };
    }

    private async Task<ScheduleCSummaryDto> BuildScheduleCSummaryAsync(
        int year,
        decimal grossReceipts,
        decimal fees,
        decimal shipping,
        decimal totalExpenses,
        decimal mileageDeduction,
        decimal netProfit,
        string currency,
        CancellationToken cancellationToken)
    {
        var startDate = new DateTime(year, 1, 1);
        var endDate = new DateTime(year, 12, 31);

        // Get expenses grouped by Schedule C line
        var expenses = await _expenseRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        var filteredExpenses = expenses
            .Where(e => e.Amount.Currency == currency && e.IsTaxDeductible)
            .ToList();

        // Build expense lines in Schedule C order
        var expenseLines = ScheduleCLineExtensions.GetAllInScheduleCOrder()
            .Select(category =>
            {
                var categoryExpenses = filteredExpenses.Where(e => e.Category == category).ToList();
                var total = categoryExpenses.Sum(e => e.Amount.Amount);

                // Special handling for Car and truck (Line 9) - add mileage deduction
                if (category == ExpenseCategory.CarAndTruck)
                {
                    total += mileageDeduction;
                }

                // Special handling for Commissions and fees (Line 10) - add eBay/payment processing fees from revenue entries
                if (category == ExpenseCategory.CommissionsAndFees)
                {
                    total += fees;
                }

                // Special handling for Supplies (Line 22) - add shipping costs from revenue entries
                if (category == ExpenseCategory.Supplies)
                {
                    total += shipping;
                }

                return new ScheduleCLineItemDto
                {
                    LineNumber = category.GetScheduleCLine(),
                    LineLabel = category.GetScheduleCLineLabel(),
                    Description = category.GetScheduleCDescription(),
                    Amount = total,
                    AmountFormatted = new Money(total, currency).ToString(),
                    ExpenseCount = categoryExpenses.Count
                };
            })
            .Where(line => line.Amount > 0) // Only include lines with expenses
            .ToList();

        // Calculate COGS from sold inventory items
        var soldInventory = await _inventoryRepository.GetByStatusAsync(
            InventoryStatus.Sold,
            cancellationToken);
        var cogsTotal = soldInventory
            .Where(i => i.SoldDate >= startDate && i.SoldDate <= endDate && i.Cogs.Currency == currency)
            .Sum(i => i.Cogs.Amount);

        var grossIncome = grossReceipts - cogsTotal;
        var line28TotalExpenses = expenseLines.Sum(l => l.Amount);

        return new ScheduleCSummaryDto
        {
            Year = year,
            Line1_GrossReceipts = grossReceipts,
            Line2_ReturnsAndAllowances = 0, // Not tracking returns currently
            Line4_CostOfGoodsSold = cogsTotal,
            Line7_GrossIncome = grossIncome,
            ExpenseLines = expenseLines,
            Line28_TotalExpenses = line28TotalExpenses,
            Line31_NetProfit = netProfit,
            Currency = currency,
            Line1Formatted = new Money(grossReceipts, currency).ToString(),
            Line3Formatted = new Money(grossReceipts, currency).ToString(),
            Line4Formatted = new Money(cogsTotal, currency).ToString(),
            Line5Formatted = new Money(grossReceipts - cogsTotal, currency).ToString(),
            Line7Formatted = new Money(grossIncome, currency).ToString(),
            Line28Formatted = new Money(line28TotalExpenses, currency).ToString(),
            Line29Formatted = new Money(grossIncome - line28TotalExpenses, currency).ToString(),
            Line31Formatted = new Money(netProfit, currency).ToString()
        };
    }

    private static (DateTime Start, DateTime End) GetQuarterDateRange(int year, int quarter)
    {
        return quarter switch
        {
            1 => (new DateTime(year, 1, 1), new DateTime(year, 3, 31)),
            2 => (new DateTime(year, 4, 1), new DateTime(year, 6, 30)),
            3 => (new DateTime(year, 7, 1), new DateTime(year, 9, 30)),
            4 => (new DateTime(year, 10, 1), new DateTime(year, 12, 31)),
            _ => throw new ArgumentOutOfRangeException(nameof(quarter), "Quarter must be 1-4")
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
