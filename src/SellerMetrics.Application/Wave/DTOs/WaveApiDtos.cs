namespace SellerMetrics.Application.Wave.DTOs;

/// <summary>
/// DTO for a Wave business.
/// </summary>
public class WaveBusinessDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsPersonal { get; set; }
    public string? Currency { get; set; }
}

/// <summary>
/// DTO for a Wave invoice from the API.
/// </summary>
public class WaveInvoiceDto
{
    public string Id { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public WaveCustomerDto? Customer { get; set; }
    public WaveMoneyDto Total { get; set; } = new();
    public WaveMoneyDto AmountDue { get; set; } = new();
    public WaveMoneyDto AmountPaid { get; set; } = new();
    public string? Memo { get; set; }
    public string? ViewUrl { get; set; }
    public List<WavePaymentDto> Payments { get; set; } = new();
    public List<WaveInvoiceItemDto> Items { get; set; } = new();
}

/// <summary>
/// DTO for a Wave customer.
/// </summary>
public class WaveCustomerDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
}

/// <summary>
/// DTO for monetary values in Wave.
/// </summary>
public class WaveMoneyDto
{
    public decimal Value { get; set; }
    public string Currency { get; set; } = "USD";
}

/// <summary>
/// DTO for a Wave payment.
/// </summary>
public class WavePaymentDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public WaveMoneyDto Amount { get; set; } = new();
    public string? PaymentMethod { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for a Wave invoice line item.
/// </summary>
public class WaveInvoiceItemDto
{
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public WaveMoneyDto UnitPrice { get; set; } = new();
    public WaveMoneyDto Total { get; set; } = new();
}
