using Contracts.Common;

namespace Domain.Models
{
    public class PagedList<T>
    {
        public PagedList()
        {

        }

        public PagedList(List<T> items, int page, int pageSize, int totalCount)
        {
            Items = items ?? [];
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public List<T> Items { get; set; } = [];
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; } = 0;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasHextPage => Page * PageSize < TotalCount;
        public bool HasPreviousPage => Page > 1;
        public List<Link> Links { get; set; } = [];
    }
}
