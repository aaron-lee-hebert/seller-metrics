namespace SellerMetrics.Domain.Enums;

/// <summary>
/// Item condition values matching eBay's condition system.
/// See: https://developer.ebay.com/devzone/finding/callref/Enums/conditionIdList.html
/// </summary>
public enum EbayCondition
{
    /// <summary>
    /// New: A brand-new, unused, unopened, undamaged item.
    /// </summary>
    New = 1000,

    /// <summary>
    /// New other: A new item with no signs of wear.
    /// May include original packaging or protective wrapping, or may be in non-original packaging.
    /// </summary>
    NewOther = 1500,

    /// <summary>
    /// New with defects: A new, unused item with defects.
    /// </summary>
    NewWithDefects = 1750,

    /// <summary>
    /// Certified Refurbished: Professionally restored to working order by a manufacturer or manufacturer-approved vendor.
    /// </summary>
    CertifiedRefurbished = 2000,

    /// <summary>
    /// Excellent - Refurbished: An item in excellent condition that has been professionally refurbished.
    /// </summary>
    ExcellentRefurbished = 2010,

    /// <summary>
    /// Very Good - Refurbished: An item in very good condition that has been professionally refurbished.
    /// </summary>
    VeryGoodRefurbished = 2020,

    /// <summary>
    /// Good - Refurbished: An item in good condition that has been professionally refurbished.
    /// </summary>
    GoodRefurbished = 2030,

    /// <summary>
    /// Seller Refurbished: An item restored to working order by the seller or a third party.
    /// </summary>
    SellerRefurbished = 2500,

    /// <summary>
    /// Like New: An item that looks like it was just taken out of shrink wrap.
    /// </summary>
    LikeNew = 2750,

    /// <summary>
    /// Used - Excellent: An item that has been used but is in excellent condition.
    /// </summary>
    UsedExcellent = 3000,

    /// <summary>
    /// Used - Very Good: An item that has been used but is in very good condition.
    /// </summary>
    UsedVeryGood = 4000,

    /// <summary>
    /// Used - Good: An item that has been used but is in good condition.
    /// </summary>
    UsedGood = 5000,

    /// <summary>
    /// Used - Acceptable: An item that has been used and shows some wear.
    /// </summary>
    UsedAcceptable = 6000,

    /// <summary>
    /// For Parts or Not Working: An item that does not function as intended.
    /// </summary>
    ForPartsOrNotWorking = 7000
}

/// <summary>
/// Extension methods for EbayCondition enum.
/// </summary>
public static class EbayConditionExtensions
{
    /// <summary>
    /// Gets a human-readable display name for the condition.
    /// </summary>
    public static string GetDisplayName(this EbayCondition condition)
    {
        return condition switch
        {
            EbayCondition.New => "New",
            EbayCondition.NewOther => "New (Other)",
            EbayCondition.NewWithDefects => "New with Defects",
            EbayCondition.CertifiedRefurbished => "Certified Refurbished",
            EbayCondition.ExcellentRefurbished => "Excellent - Refurbished",
            EbayCondition.VeryGoodRefurbished => "Very Good - Refurbished",
            EbayCondition.GoodRefurbished => "Good - Refurbished",
            EbayCondition.SellerRefurbished => "Seller Refurbished",
            EbayCondition.LikeNew => "Like New",
            EbayCondition.UsedExcellent => "Used - Excellent",
            EbayCondition.UsedVeryGood => "Used - Very Good",
            EbayCondition.UsedGood => "Used - Good",
            EbayCondition.UsedAcceptable => "Used - Acceptable",
            EbayCondition.ForPartsOrNotWorking => "For Parts or Not Working",
            _ => condition.ToString()
        };
    }
}
