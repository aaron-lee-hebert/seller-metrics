using SellerMetrics.Domain.Entities;
using SellerMetrics.Domain.Enums;

namespace SellerMetrics.Domain.Interfaces;

/// <summary>
/// Repository interface for EbayOrder with specialized operations.
/// </summary>
public interface IEbayOrderRepository : IRepository<EbayOrder>
{
    /// <summary>
    /// Gets an order by its eBay order ID.
    /// </summary>
    /// <param name="ebayOrderId">The eBay order ID.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The order if found; otherwise, null.</returns>
    Task<EbayOrder?> GetByEbayOrderIdAsync(
        string ebayOrderId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets orders for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of orders for the user.</returns>
    Task<IReadOnlyList<EbayOrder>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets orders for a user within a date range.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="startDate">The start date (inclusive).</param>
    /// <param name="endDate">The end date (inclusive).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of orders within the date range.</returns>
    Task<IReadOnlyList<EbayOrder>> GetByDateRangeAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets orders by status for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="status">The order status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of orders with the specified status.</returns>
    Task<IReadOnlyList<EbayOrder>> GetByStatusAsync(
        string userId,
        EbayOrderStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets orders linked to a specific inventory item.
    /// </summary>
    /// <param name="inventoryItemId">The inventory item ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of orders linked to the inventory item.</returns>
    Task<IReadOnlyList<EbayOrder>> GetByInventoryItemIdAsync(
        int inventoryItemId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets unlinked orders (orders without a linked inventory item) for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of unlinked orders.</returns>
    Task<IReadOnlyList<EbayOrder>> GetUnlinkedOrdersAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total gross sales for a user within a date range.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="startDate">The start date (inclusive).</param>
    /// <param name="endDate">The end date (inclusive).</param>
    /// <param name="currency">The currency code (defaults to USD).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The total gross sales amount.</returns>
    Task<decimal> GetTotalGrossSalesAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total profit for a user within a date range.
    /// Only includes orders with linked inventory items.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="startDate">The start date (inclusive).</param>
    /// <param name="endDate">The end date (inclusive).</param>
    /// <param name="currency">The currency code (defaults to USD).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The total profit amount.</returns>
    Task<decimal> GetTotalProfitAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        string currency = "USD",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets order count for a user within a date range.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="startDate">The start date (inclusive).</param>
    /// <param name="endDate">The end date (inclusive).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The count of orders.</returns>
    Task<int> GetOrderCountAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most recent orders for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="count">The number of orders to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of the most recent orders.</returns>
    Task<IReadOnlyList<EbayOrder>> GetRecentOrdersAsync(
        string userId,
        int count = 5,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an eBay order already exists for a user.
    /// </summary>
    /// <param name="ebayOrderId">The eBay order ID.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the order exists.</returns>
    Task<bool> ExistsAsync(
        string ebayOrderId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an eBay order was previously deleted (soft-deleted).
    /// Used during sync to avoid re-creating deleted orders.
    /// </summary>
    /// <param name="ebayOrderId">The eBay order ID.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the order was previously deleted.</returns>
    Task<bool> WasDeletedAsync(
        string ebayOrderId,
        string userId,
        CancellationToken cancellationToken = default);
}
