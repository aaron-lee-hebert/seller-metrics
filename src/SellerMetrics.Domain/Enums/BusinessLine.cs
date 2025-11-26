namespace SellerMetrics.Domain.Enums;

/// <summary>
/// Business line assignment for expenses and mileage.
/// </summary>
public enum BusinessLine
{
    /// <summary>
    /// Expense related to eBay reselling business.
    /// </summary>
    eBay = 1,

    /// <summary>
    /// Expense related to computer support services.
    /// </summary>
    ComputerServices = 2,

    /// <summary>
    /// Expense shared between both business lines.
    /// Will be allocated proportionally for tax reporting.
    /// </summary>
    Shared = 3
}
