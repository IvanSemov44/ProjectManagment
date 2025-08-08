namespace Contracts.Requests
{
    public class ProjectRequestParameters : RequestParameters
    {
        private const int DefaultPage = 1;
        private const int DefaultPageSize = 5;

        new public int Page => base.Page ?? DefaultPage;
        new public int PageSize => base.PageSize ?? DefaultPageSize;
        public string? Name { get; init; }
    }
}
