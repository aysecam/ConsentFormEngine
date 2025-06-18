using ConsentFormEngine.Core.Shared;

namespace ConsentFormEngine.Business.DTOs
{
    public class GetFormEntryReportRequestDto
    {
        public PageRequest? PageRequest { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
