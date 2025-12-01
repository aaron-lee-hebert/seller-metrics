using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using SellerMetrics.Application.Wave.DTOs;
using SellerMetrics.Application.Wave.Interfaces;

namespace SellerMetrics.Infrastructure.Services;

/// <summary>
/// Wave API client implementation using GraphQL.
/// </summary>
public class WaveApiClient : IWaveApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WaveApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    private const string WaveGraphQLEndpoint = "https://gql.waveapps.com/graphql/public";

    public WaveApiClient(
        HttpClient httpClient,
        ILogger<WaveApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<IReadOnlyList<WaveBusinessDto>> GetBusinessesAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        const string query = @"
            query {
                businesses(page: 1, pageSize: 100) {
                    edges {
                        node {
                            id
                            name
                            isPersonal
                            currency {
                                code
                            }
                        }
                    }
                }
            }";

        var response = await ExecuteGraphQLAsync<BusinessesResponse>(query, accessToken, cancellationToken);

        return response?.Data?.Businesses?.Edges?
            .Select(e => new WaveBusinessDto
            {
                Id = e.Node?.Id ?? string.Empty,
                Name = e.Node?.Name ?? string.Empty,
                IsPersonal = e.Node?.IsPersonal ?? false,
                Currency = e.Node?.Currency?.Code
            })
            .ToList() ?? new List<WaveBusinessDto>();
    }

    public async Task<IReadOnlyList<WaveInvoiceDto>> GetInvoicesAsync(
        string accessToken,
        string businessId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var allInvoices = new List<WaveInvoiceDto>();
        var page = 1;
        const int pageSize = 50;
        var hasMore = true;

        while (hasMore)
        {
            var query = $@"
                query {{
                    business(id: ""{businessId}"") {{
                        invoices(page: {page}, pageSize: {pageSize}) {{
                            pageInfo {{
                                totalPages
                                currentPage
                            }}
                            edges {{
                                node {{
                                    id
                                    invoiceNumber
                                    status
                                    invoiceDate
                                    dueDate
                                    memo
                                    viewUrl
                                    customer {{
                                        id
                                        name
                                        email
                                    }}
                                    total {{
                                        value
                                        currency {{
                                            code
                                        }}
                                    }}
                                    amountDue {{
                                        value
                                        currency {{
                                            code
                                        }}
                                    }}
                                    amountPaid {{
                                        value
                                        currency {{
                                            code
                                        }}
                                    }}
                                }}
                            }}
                        }}
                    }}
                }}";

            var response = await ExecuteGraphQLAsync<InvoicesResponse>(query, accessToken, cancellationToken);
            var pageInfo = response?.Data?.Business?.Invoices?.PageInfo;
            var edges = response?.Data?.Business?.Invoices?.Edges ?? new List<InvoiceEdge>();

            foreach (var edge in edges)
            {
                var node = edge.Node;
                if (node == null) continue;

                var invoiceDate = DateTime.TryParse(node.InvoiceDate, out var parsedDate)
                    ? parsedDate
                    : DateTime.MinValue;

                // Filter by date range
                if (invoiceDate < startDate || invoiceDate > endDate)
                    continue;

                allInvoices.Add(MapToInvoiceDto(node));
            }

            // Check for more pages
            hasMore = pageInfo != null && pageInfo.CurrentPage < pageInfo.TotalPages;
            page++;

            // Safety limit
            if (page > 20)
            {
                _logger.LogWarning("Reached safety limit of 20 pages during Wave invoice sync");
                break;
            }
        }

        return allInvoices;
    }

    public async Task<WaveInvoiceDto?> GetInvoiceAsync(
        string accessToken,
        string businessId,
        string invoiceId,
        CancellationToken cancellationToken = default)
    {
        var query = $@"
            query {{
                business(id: ""{businessId}"") {{
                    invoice(id: ""{invoiceId}"") {{
                        id
                        invoiceNumber
                        status
                        invoiceDate
                        dueDate
                        memo
                        viewUrl
                        customer {{
                            id
                            name
                            email
                        }}
                        total {{
                            value
                            currency {{
                                code
                            }}
                        }}
                        amountDue {{
                            value
                            currency {{
                                code
                            }}
                        }}
                        amountPaid {{
                            value
                            currency {{
                                code
                            }}
                        }}
                        items {{
                            description
                            quantity
                            unitPrice {{
                                value
                                currency {{
                                    code
                                }}
                            }}
                            total {{
                                value
                                currency {{
                                    code
                                }}
                            }}
                        }}
                    }}
                }}
            }}";

        var response = await ExecuteGraphQLAsync<SingleInvoiceResponse>(query, accessToken, cancellationToken);
        var node = response?.Data?.Business?.Invoice;

        return node != null ? MapToInvoiceDto(node) : null;
    }

    public async Task<bool> ValidateTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var businesses = await GetBusinessesAsync(accessToken, cancellationToken);
            return businesses.Count > 0;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Wave token validation failed");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during Wave token validation");
            return false;
        }
    }

    private async Task<T?> ExecuteGraphQLAsync<T>(
        string query,
        string accessToken,
        CancellationToken cancellationToken)
    {
        var requestBody = new { query };
        var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, WaveGraphQLEndpoint)
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Wave API request failed: {StatusCode} - {Content}",
                response.StatusCode, responseContent);
            throw new HttpRequestException($"Wave API request failed: {response.StatusCode}");
        }

        return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
    }

    private static WaveInvoiceDto MapToInvoiceDto(InvoiceNode node)
    {
        return new WaveInvoiceDto
        {
            Id = node.Id ?? string.Empty,
            InvoiceNumber = node.InvoiceNumber ?? string.Empty,
            Status = node.Status ?? "DRAFT",
            InvoiceDate = DateTime.TryParse(node.InvoiceDate, out var invoiceDate)
                ? invoiceDate
                : DateTime.MinValue,
            DueDate = DateTime.TryParse(node.DueDate, out var dueDate)
                ? dueDate
                : DateTime.MinValue,
            Memo = node.Memo,
            ViewUrl = node.ViewUrl,
            Customer = node.Customer != null
                ? new WaveCustomerDto
                {
                    Id = node.Customer.Id ?? string.Empty,
                    Name = node.Customer.Name ?? "Unknown",
                    Email = node.Customer.Email
                }
                : null,
            Total = new WaveMoneyDto
            {
                Value = node.Total?.Value ?? 0,
                Currency = node.Total?.Currency?.Code ?? "USD"
            },
            AmountDue = new WaveMoneyDto
            {
                Value = node.AmountDue?.Value ?? 0,
                Currency = node.AmountDue?.Currency?.Code ?? "USD"
            },
            AmountPaid = new WaveMoneyDto
            {
                Value = node.AmountPaid?.Value ?? 0,
                Currency = node.AmountPaid?.Currency?.Code ?? "USD"
            },
            Items = node.Items?.Select(i => new WaveInvoiceItemDto
            {
                Description = i.Description,
                Quantity = i.Quantity ?? 1,
                UnitPrice = new WaveMoneyDto
                {
                    Value = i.UnitPrice?.Value ?? 0,
                    Currency = i.UnitPrice?.Currency?.Code ?? "USD"
                },
                Total = new WaveMoneyDto
                {
                    Value = i.Total?.Value ?? 0,
                    Currency = i.Total?.Currency?.Code ?? "USD"
                }
            }).ToList() ?? new List<WaveInvoiceItemDto>()
        };
    }

    #region GraphQL Response Models

    private class BusinessesResponse
    {
        public BusinessesData? Data { get; set; }
    }

    private class BusinessesData
    {
        public BusinessConnection? Businesses { get; set; }
    }

    private class BusinessConnection
    {
        public List<BusinessEdge>? Edges { get; set; }
    }

    private class BusinessEdge
    {
        public BusinessNode? Node { get; set; }
    }

    private class BusinessNode
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public bool IsPersonal { get; set; }
        public CurrencyNode? Currency { get; set; }
    }

    private class CurrencyNode
    {
        public string? Code { get; set; }
    }

    private class InvoicesResponse
    {
        public InvoicesData? Data { get; set; }
    }

    private class InvoicesData
    {
        public BusinessInvoices? Business { get; set; }
    }

    private class BusinessInvoices
    {
        public InvoiceConnection? Invoices { get; set; }
    }

    private class InvoiceConnection
    {
        public PageInfo? PageInfo { get; set; }
        public List<InvoiceEdge>? Edges { get; set; }
    }

    private class PageInfo
    {
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }

    private class InvoiceEdge
    {
        public InvoiceNode? Node { get; set; }
    }

    private class SingleInvoiceResponse
    {
        public SingleInvoiceData? Data { get; set; }
    }

    private class SingleInvoiceData
    {
        public SingleInvoiceBusiness? Business { get; set; }
    }

    private class SingleInvoiceBusiness
    {
        public InvoiceNode? Invoice { get; set; }
    }

    private class InvoiceNode
    {
        public string? Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? Status { get; set; }
        public string? InvoiceDate { get; set; }
        public string? DueDate { get; set; }
        public string? Memo { get; set; }
        public string? ViewUrl { get; set; }
        public CustomerNode? Customer { get; set; }
        public MoneyNode? Total { get; set; }
        public MoneyNode? AmountDue { get; set; }
        public MoneyNode? AmountPaid { get; set; }
        public List<InvoiceItemNode>? Items { get; set; }
    }

    private class CustomerNode
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }

    private class MoneyNode
    {
        public decimal Value { get; set; }
        public CurrencyNode? Currency { get; set; }
    }

    private class InvoiceItemNode
    {
        public string? Description { get; set; }
        public int? Quantity { get; set; }
        public MoneyNode? UnitPrice { get; set; }
        public MoneyNode? Total { get; set; }
    }

    #endregion
}
