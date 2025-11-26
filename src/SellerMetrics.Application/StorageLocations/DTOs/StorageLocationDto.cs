namespace SellerMetrics.Application.StorageLocations.DTOs;

/// <summary>
/// Data transfer object for StorageLocation.
/// </summary>
public class StorageLocationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public string? ParentName { get; set; }
    public string FullPath { get; set; } = string.Empty;
    public int Depth { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<StorageLocationDto> Children { get; set; } = new();
}

/// <summary>
/// Summary DTO for dropdowns and simple listings.
/// </summary>
public class StorageLocationSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public int Depth { get; set; }
}
