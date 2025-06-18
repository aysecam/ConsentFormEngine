
namespace ConsentFormEngine.Core.Shared
{
    public class PagedList<T>
    {
        public List<T> Items { get; }
        public int TotalCount { get; }
        public int PageIndex { get; }
        public int PageSize { get; }

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPrevious => PageIndex > 0;
        public bool HasNext => PageIndex + 1 < TotalPages;
        public bool IsEmpty => Items.Count == 0;

        public PagedList(List<T> items, int totalCount, int pageIndex, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }

}
