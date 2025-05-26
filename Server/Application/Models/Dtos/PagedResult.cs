using System.Text.Json.Serialization;

namespace Application.Models.Dtos;

public class PagedResult<T>
{
    [JsonPropertyName("items")]
    public List<T> Items { get; set; } = new List<T>();
    
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; }
    
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }
    
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }
    
    [JsonPropertyName("totalPages")]
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}