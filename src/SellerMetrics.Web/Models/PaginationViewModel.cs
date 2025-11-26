namespace SellerMetrics.Web.Models;

/// <summary>
/// ViewModel for server-side pagination.
/// </summary>
public class PaginationViewModel
{
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalItems { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public Dictionary<string, string> QueryParameters { get; set; } = new();

    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public int StartItem => TotalItems == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;
    public int EndItem => Math.Min(CurrentPage * PageSize, TotalItems);

    public string GetPageUrl(int pageNumber)
    {
        var parameters = new Dictionary<string, string>(QueryParameters)
        {
            ["page"] = pageNumber.ToString()
        };

        var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
        return string.IsNullOrEmpty(queryString) ? BaseUrl : $"{BaseUrl}?{queryString}";
    }

    public static PaginationViewModel Create<T>(
        IEnumerable<T> items,
        int currentPage,
        int pageSize,
        int totalItems,
        string baseUrl,
        Dictionary<string, string>? queryParameters = null)
    {
        return new PaginationViewModel
        {
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalItems = totalItems,
            BaseUrl = baseUrl,
            QueryParameters = queryParameters ?? new Dictionary<string, string>()
        };
    }
}

/// <summary>
/// Generic paginated list for view models.
/// </summary>
public class PaginatedList<T>
{
    public List<T> Items { get; }
    public int CurrentPage { get; }
    public int PageSize { get; }
    public int TotalItems { get; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public PaginatedList(List<T> items, int currentPage, int pageSize, int totalItems)
    {
        Items = items;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalItems = totalItems;
    }

    public static PaginatedList<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var items = source.ToList();
        var totalItems = items.Count;
        var pagedItems = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(pagedItems, pageNumber, pageSize, totalItems);
    }
}
