namespace SellerMetrics.Domain.Enums;

/// <summary>
/// IRS Schedule C expense categories for sole proprietorship tax reporting.
/// </summary>
public enum ExpenseCategory
{
    /// <summary>
    /// Shipping supplies: boxes, tape, bubble wrap, labels, etc.
    /// </summary>
    ShippingSupplies = 1,

    /// <summary>
    /// Office supplies: paper, pens, printer ink, etc.
    /// </summary>
    OfficeSupplies = 2,

    /// <summary>
    /// Advertising and marketing costs.
    /// </summary>
    AdvertisingMarketing = 3,

    /// <summary>
    /// Professional services: accounting, legal, consulting.
    /// </summary>
    ProfessionalServices = 4,

    /// <summary>
    /// Vehicle and mileage expenses (tracked separately in MileageLog).
    /// </summary>
    VehicleMileage = 5,

    /// <summary>
    /// Tools and equipment purchases.
    /// </summary>
    ToolsEquipment = 6,

    /// <summary>
    /// Software subscriptions and digital services.
    /// </summary>
    SoftwareSubscriptions = 7,

    /// <summary>
    /// Parts and materials for repairs/services.
    /// </summary>
    PartsMaterials = 8,

    /// <summary>
    /// Postage and shipping costs.
    /// </summary>
    PostageShipping = 9,

    /// <summary>
    /// Business insurance premiums.
    /// </summary>
    Insurance = 10,

    /// <summary>
    /// Interest on business loans or credit.
    /// </summary>
    Interest = 11,

    /// <summary>
    /// Bank fees and payment processing fees.
    /// </summary>
    BankFees = 12,

    /// <summary>
    /// Education, training, and certification costs.
    /// </summary>
    EducationTraining = 13,

    /// <summary>
    /// Utilities (if home office deduction applies).
    /// </summary>
    Utilities = 14,

    /// <summary>
    /// Rent (if applicable for storage or office space).
    /// </summary>
    Rent = 15,

    /// <summary>
    /// Other business expenses not categorized above.
    /// </summary>
    Other = 99
}
