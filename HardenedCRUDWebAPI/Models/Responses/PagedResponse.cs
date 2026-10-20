namespace ProductsApi.Models.Responses;

public record PageMeta(int Page, int PageSize, int TotalItems, int TotalPages);

// The { "data": [...], "meta": {...} } envelope used by the list endpoint
public record PagedResponse<T>(IReadOnlyList<T> Data, PageMeta Meta);
