namespace SellerMetrics.Application.Components.DTOs;

/// <summary>
/// Data transfer object for ComponentType.
/// </summary>
public class ComponentTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DefaultExpenseCategory { get; set; }
    public bool IsSystemType { get; set; }
    public int SortOrder { get; set; }
}
