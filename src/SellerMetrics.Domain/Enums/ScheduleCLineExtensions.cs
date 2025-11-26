namespace SellerMetrics.Domain.Enums;

/// <summary>
/// Extension methods for ExpenseCategory to provide IRS Schedule C line information.
/// </summary>
public static class ScheduleCLineExtensions
{
    /// <summary>
    /// Gets the IRS Schedule C line number for the expense category.
    /// </summary>
    public static int GetScheduleCLine(this ExpenseCategory category)
    {
        return (int)category;
    }

    /// <summary>
    /// Gets the IRS Schedule C line label (e.g., "Line 8", "Line 17").
    /// </summary>
    public static string GetScheduleCLineLabel(this ExpenseCategory category)
    {
        return $"Line {(int)category}";
    }

    /// <summary>
    /// Gets the official IRS Schedule C line description.
    /// </summary>
    public static string GetScheduleCDescription(this ExpenseCategory category)
    {
        return category switch
        {
            ExpenseCategory.Advertising => "Advertising",
            ExpenseCategory.CarAndTruck => "Car and truck expenses",
            ExpenseCategory.CommissionsAndFees => "Commissions and fees",
            ExpenseCategory.ContractLabor => "Contract labor",
            ExpenseCategory.Depreciation => "Depreciation and section 179 expense deduction",
            ExpenseCategory.Insurance => "Insurance (other than health)",
            ExpenseCategory.Interest => "Interest (other)",
            ExpenseCategory.LegalAndProfessional => "Legal and professional services",
            ExpenseCategory.OfficeExpense => "Office expense",
            ExpenseCategory.RentOrLease => "Rent or lease (other business property)",
            ExpenseCategory.RepairsAndMaintenance => "Repairs and maintenance",
            ExpenseCategory.Supplies => "Supplies",
            ExpenseCategory.TaxesAndLicenses => "Taxes and licenses",
            ExpenseCategory.Travel => "Travel",
            ExpenseCategory.Utilities => "Utilities",
            ExpenseCategory.OtherExpenses => "Other expenses",
            _ => "Other expenses"
        };
    }

    /// <summary>
    /// Gets a user-friendly display name for the expense category.
    /// </summary>
    public static string GetDisplayName(this ExpenseCategory category)
    {
        return category switch
        {
            ExpenseCategory.Advertising => "Advertising & Marketing",
            ExpenseCategory.CarAndTruck => "Car & Truck Expenses",
            ExpenseCategory.CommissionsAndFees => "Commissions & Fees",
            ExpenseCategory.ContractLabor => "Contract Labor",
            ExpenseCategory.Depreciation => "Depreciation",
            ExpenseCategory.Insurance => "Insurance",
            ExpenseCategory.Interest => "Interest",
            ExpenseCategory.LegalAndProfessional => "Legal & Professional",
            ExpenseCategory.OfficeExpense => "Office Expense",
            ExpenseCategory.RentOrLease => "Rent or Lease",
            ExpenseCategory.RepairsAndMaintenance => "Repairs & Maintenance",
            ExpenseCategory.Supplies => "Supplies",
            ExpenseCategory.TaxesAndLicenses => "Taxes & Licenses",
            ExpenseCategory.Travel => "Travel",
            ExpenseCategory.Utilities => "Utilities",
            ExpenseCategory.OtherExpenses => "Other Expenses",
            _ => "Other Expenses"
        };
    }

    /// <summary>
    /// Gets all expense categories in Schedule C line order.
    /// </summary>
    public static IReadOnlyList<ExpenseCategory> GetAllInScheduleCOrder()
    {
        return new[]
        {
            ExpenseCategory.Advertising,
            ExpenseCategory.CarAndTruck,
            ExpenseCategory.CommissionsAndFees,
            ExpenseCategory.ContractLabor,
            ExpenseCategory.Depreciation,
            ExpenseCategory.Insurance,
            ExpenseCategory.Interest,
            ExpenseCategory.LegalAndProfessional,
            ExpenseCategory.OfficeExpense,
            ExpenseCategory.RentOrLease,
            ExpenseCategory.RepairsAndMaintenance,
            ExpenseCategory.Supplies,
            ExpenseCategory.TaxesAndLicenses,
            ExpenseCategory.Travel,
            ExpenseCategory.Utilities,
            ExpenseCategory.OtherExpenses
        };
    }
}
