namespace SellerMetrics.Domain.Enums;

/// <summary>
/// IRS Schedule C expense categories for sole proprietorship tax reporting.
/// Values correspond to Schedule C Part II line numbers where applicable.
/// </summary>
public enum ExpenseCategory
{
    /// <summary>
    /// Line 8: Advertising and marketing costs (eBay promoted listings, business cards, etc.).
    /// </summary>
    Advertising = 8,

    /// <summary>
    /// Line 9: Car and truck expenses (tracked separately in MileageLog for standard mileage method).
    /// </summary>
    CarAndTruck = 9,

    /// <summary>
    /// Line 10: Commissions and fees (eBay fees, PayPal fees, platform fees).
    /// </summary>
    CommissionsAndFees = 10,

    /// <summary>
    /// Line 11: Contract labor (freelancers, contractors).
    /// </summary>
    ContractLabor = 11,

    /// <summary>
    /// Line 13: Depreciation and Section 179 expense (equipment, computers).
    /// </summary>
    Depreciation = 13,

    /// <summary>
    /// Line 15: Insurance (other than health) - business liability, property insurance.
    /// </summary>
    Insurance = 15,

    /// <summary>
    /// Line 16b: Interest (other) - business credit cards, loans.
    /// </summary>
    Interest = 16,

    /// <summary>
    /// Line 17: Legal and professional services (accounting, legal fees, tax prep).
    /// </summary>
    LegalAndProfessional = 17,

    /// <summary>
    /// Line 18: Office expense (paper, pens, printer ink, small office items).
    /// </summary>
    OfficeExpense = 18,

    /// <summary>
    /// Line 20b: Rent or lease (other business property) - storage units, office space.
    /// </summary>
    RentOrLease = 20,

    /// <summary>
    /// Line 21: Repairs and maintenance (equipment repairs, computer repairs).
    /// </summary>
    RepairsAndMaintenance = 21,

    /// <summary>
    /// Line 22: Supplies (shipping supplies, packaging materials, parts for resale repairs).
    /// </summary>
    Supplies = 22,

    /// <summary>
    /// Line 23: Taxes and licenses (business licenses, sales tax permits).
    /// </summary>
    TaxesAndLicenses = 23,

    /// <summary>
    /// Line 24a: Travel (business travel, lodging for sourcing trips).
    /// </summary>
    Travel = 24,

    /// <summary>
    /// Line 25: Utilities (business portion of utilities if home office).
    /// </summary>
    Utilities = 25,

    /// <summary>
    /// Line 27a: Other expenses - education/training, software subscriptions,
    /// bank fees, and other deductible business expenses.
    /// </summary>
    OtherExpenses = 27
}
