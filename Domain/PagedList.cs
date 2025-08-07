namespace Domain
{
    public class PagedList<T>(IEnumerable<T> items, int page, int pageSize, int totalCount)
    {
        public IEnumerable<T> Items => items;
        public int Page => page;
        public int PageSize => pageSize;
        public int TotalCount => totalCount;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / pageSize);
        public bool HasHextPage => Page * PageSize < TotalCount;
        public bool HasPreviousPage => Page > 1;
    }
}
