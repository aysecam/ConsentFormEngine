using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using ConsentFormEngine.Business.Abstract;
using ConsentFormEngine.Business.DTOs;
using ConsentFormEngine.Core.Shared;
using ConsentFormEngine.Core.Utilities;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace ConsentFormEngine.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormEntryController : ControllerBase
    {
        private readonly IFormEntryService _formEntryService;


        public FormEntryController(IFormEntryService formEntryService)
        {
            _formEntryService = formEntryService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitForm([FromBody] FormEntryCreateRequestDto dto)
        {
            var result = await _formEntryService.SubmitFormAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        [Authorize]
        [HttpPost("report")]
        public async Task<ActionResult<DataResult<PagedList<GetFormEntryReportResponseDto>>>> GetFormEntryReport([FromBody] GetFormEntryReportRequestDto dto)
        {
            var result = await _formEntryService.GetFormEnrtyReport(dto);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        [Authorize]
        [HttpPost("export-csv")]
        public async Task<IActionResult> ExportFormEntryReportToCsv([FromBody] GetFormEntryReportRequestDto dto)
        {
            var result = await _formEntryService.GetFormEnrtyReport(dto);

            if (!result.Success || result.Data == null || result.Data.Items.Count == 0)
                return NotFound("Kayıt bulunamadı.");

            var memoryStream = new MemoryStream();
            var encoding = new UTF8Encoding(true); // BOM'lu UTF-8

            using (var streamWriter = new StreamWriter(memoryStream, encoding, leaveOpen: true))
            using (var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.GetCultureInfo("tr-TR")) { Delimiter = "," }))
            {
                csvWriter.WriteHeader<GetFormEntryReportResponseDto>();
                await csvWriter.NextRecordAsync();

                foreach (var item in result.Data.Items)
                {
                    csvWriter.WriteRecord(item);
                    await csvWriter.NextRecordAsync();
                }
            }

            memoryStream.Position = 0;
            return File(memoryStream.ToArray(), "application/octet-stream", $"form-entry-report_{DateTime.Now:yyyyMMddHHmmss}.csv");
        }
    }

}
