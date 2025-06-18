namespace ConsentFormEngine.Core.Shared
{
    public class PageRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }
    }
}
