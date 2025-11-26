namespace SellerMetrics.Domain.Enums;

/// <summary>
/// How a component was acquired.
/// </summary>
public enum ComponentSource
{
    /// <summary>
    /// Component was purchased from a vendor or supplier.
    /// </summary>
    Purchased = 0,

    /// <summary>
    /// Component was salvaged from existing equipment.
    /// </summary>
    Salvaged = 1,

    /// <summary>
    /// Component was provided by the customer for their repair.
    /// </summary>
    CustomerProvided = 2
}
