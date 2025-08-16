namespace Contracts.Requests
{
    public abstract class RequestParameters
    {
        public int? Page { get; init; }
        public int? PageSize { get; init; }
        public string? SearchTerm { get; init; }
        public string? SortBy { get; init; }
        public string? SortOrder { get; init; }
        public string? Properties { get; init; }
    }
}