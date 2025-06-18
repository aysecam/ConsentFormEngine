using ConsentFormEngine.Business.DTOs;
using ConsentFormEngine.Core.Shared;
using ConsentFormEngine.Core.Utilities;

namespace ConsentFormEngine.Business.Abstract
{
    public interface IFormEntryService
    {
        Task<Result> SubmitFormAsync(FormEntryCreateRequestDto dto);
        Task<DataResult<PagedList<GetFormEntryReportResponseDto>>> GetFormEnrtyReport(GetFormEntryReportRequestDto dto);
    }
}
