using SellerMetrics.Application.Expenses.DTOs;
using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Interfaces;

namespace SellerMetrics.Application.Expenses.Queries;

/// <summary>
/// Query to get a single expense by ID.
/// </summary>
public record GetExpenseQuery(int Id);

/// <summary>
/// Handler for GetExpenseQuery.
/// </summary>
public class GetExpenseQueryHandler
{
    private readonly IBusinessExpenseRepository _repository;

    public GetExpenseQueryHandler(IBusinessExpenseRepository repository)
    {
        _repository = repository;
    }

    public async Task<BusinessExpenseDto?> HandleAsync(
        GetExpenseQuery query,
        CancellationToken cancellationToken = default)
    {
        var expense = await _repository.GetByIdAsync(query.Id, cancellationToken);
        if (expense == null)
        {
            return null;
        }

        return MapToDto(expense);
    }

    internal static BusinessExpenseDto MapToDto(BusinessExpense expense)
    {
        return new BusinessExpenseDto
        {
            Id = expense.Id,
            ExpenseDate = expense.ExpenseDate,
            Description = expense.Description,
            Amount = expense.Amount.Amount,
            Currency = expense.Amount.Currency,
            AmountFormatted = expense.Amount.ToString(),
            Category = expense.Category,
            CategoryDisplay = FormatCategoryName(expense.Category),
            BusinessLine = expense.BusinessLine,
            BusinessLineDisplay = expense.BusinessLine == Domain.Enums.BusinessLine.eBay
                ? "eBay"
                : expense.BusinessLine.ToString(),
            Vendor = expense.Vendor,
            ReceiptPath = expense.ReceiptPath,
            Notes = expense.Notes,
            ServiceJobId = expense.ServiceJobId,
            ServiceJobNumber = expense.ServiceJob?.JobNumber,
            IsTaxDeductible = expense.IsTaxDeductible,
            ReferenceNumber = expense.ReferenceNumber,
            CreatedAt = expense.CreatedAt,
            UpdatedAt = expense.UpdatedAt
        };
    }

    private static string FormatCategoryName(Domain.Enums.ExpenseCategory category)
    {
        return category switch
        {
            Domain.Enums.ExpenseCategory.ShippingSupplies => "Shipping Supplies",
            Domain.Enums.ExpenseCategory.OfficeSupplies => "Office Supplies",
            Domain.Enums.ExpenseCategory.AdvertisingMarketing => "Advertising/Marketing",
            Domain.Enums.ExpenseCategory.ProfessionalServices => "Professional Services",
            Domain.Enums.ExpenseCategory.VehicleMileage => "Vehicle/Mileage",
            Domain.Enums.ExpenseCategory.ToolsEquipment => "Tools & Equipment",
            Domain.Enums.ExpenseCategory.SoftwareSubscriptions => "Software/Subscriptions",
            Domain.Enums.ExpenseCategory.PartsMaterials => "Parts & Materials",
            Domain.Enums.ExpenseCategory.PostageShipping => "Postage & Shipping",
            Domain.Enums.ExpenseCategory.Insurance => "Insurance",
            Domain.Enums.ExpenseCategory.Interest => "Interest",
            Domain.Enums.ExpenseCategory.BankFees => "Bank Fees",
            Domain.Enums.ExpenseCategory.EducationTraining => "Education/Training",
            Domain.Enums.ExpenseCategory.Utilities => "Utilities",
            Domain.Enums.ExpenseCategory.Rent => "Rent",
            Domain.Enums.ExpenseCategory.Other => "Other",
            _ => category.ToString()
        };
    }
}
