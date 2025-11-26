namespace SellerMetrics.Domain.ValueObjects;

/// <summary>
/// Value object representing a monetary amount with currency.
/// Implements proper value equality semantics.
/// </summary>
public sealed class Money : IEquatable<Money>
{
    /// <summary>
    /// The monetary amount. Always stored with full precision.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// The ISO 4217 currency code (e.g., "USD", "EUR", "GBP").
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// Creates a new Money instance.
    /// </summary>
    /// <param name="amount">The monetary amount.</param>
    /// <param name="currency">The ISO 4217 currency code (defaults to USD).</param>
    public Money(decimal amount, string currency = "USD")
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency code cannot be null or empty.", nameof(currency));

        if (currency.Length != 3)
            throw new ArgumentException("Currency code must be a 3-letter ISO 4217 code.", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    /// <summary>
    /// Creates a zero-value Money instance.
    /// </summary>
    /// <param name="currency">The ISO 4217 currency code (defaults to USD).</param>
    public static Money Zero(string currency = "USD") => new(0, currency);

    /// <summary>
    /// Creates a Money instance from a USD amount.
    /// </summary>
    public static Money FromUsd(decimal amount) => new(amount, "USD");

    /// <summary>
    /// Adds two Money instances. Must have the same currency.
    /// </summary>
    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    /// <summary>
    /// Subtracts another Money instance. Must have the same currency.
    /// </summary>
    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    /// <summary>
    /// Multiplies the amount by a factor.
    /// </summary>
    public Money Multiply(decimal factor) => new(Amount * factor, Currency);

    /// <summary>
    /// Returns the negation of this money amount.
    /// </summary>
    public Money Negate() => new(-Amount, Currency);

    /// <summary>
    /// Returns true if the amount is greater than zero.
    /// </summary>
    public bool IsPositive => Amount > 0;

    /// <summary>
    /// Returns true if the amount is less than zero.
    /// </summary>
    public bool IsNegative => Amount < 0;

    /// <summary>
    /// Returns true if the amount is zero.
    /// </summary>
    public bool IsZero => Amount == 0;

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException(
                $"Cannot perform operation on Money with different currencies: {Currency} and {other.Currency}");
    }

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator *(Money left, decimal right) => left.Multiply(right);
    public static Money operator -(Money money) => money.Negate();

    public static bool operator ==(Money? left, Money? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(Money? left, Money? right) => !(left == right);

    public static bool operator <(Money left, Money right)
    {
        left.EnsureSameCurrency(right);
        return left.Amount < right.Amount;
    }

    public static bool operator >(Money left, Money right)
    {
        left.EnsureSameCurrency(right);
        return left.Amount > right.Amount;
    }

    public static bool operator <=(Money left, Money right)
    {
        left.EnsureSameCurrency(right);
        return left.Amount <= right.Amount;
    }

    public static bool operator >=(Money left, Money right)
    {
        left.EnsureSameCurrency(right);
        return left.Amount >= right.Amount;
    }

    public bool Equals(Money? other)
    {
        if (other is null) return false;
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object? obj) => obj is Money other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    /// <summary>
    /// Returns a formatted string representation (e.g., "$100.00" for USD).
    /// </summary>
    public override string ToString()
    {
        return Currency switch
        {
            "USD" => $"${Amount:N2}",
            "EUR" => $"€{Amount:N2}",
            "GBP" => $"£{Amount:N2}",
            _ => $"{Amount:N2} {Currency}"
        };
    }
}
