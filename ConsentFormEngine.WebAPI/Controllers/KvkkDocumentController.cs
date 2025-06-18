using Microsoft.AspNetCore.Mvc;
using ConsentFormEngine.Core.Utilities;
using System.Text;
using UglyToad.PdfPig;

namespace ConsentFormEngine.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KvkkDocumentController : ControllerBase
    {
        private static string? _cachedKvkkHtml;

        [HttpGet("kvkk-link")]
        public IActionResult GetDocumentLink()
        {
            var url = $"{Request.Scheme}://{Request.Host}/docs/kvkk_sample_unicode.pdf";
            return Ok(new DataResult<string>(url, true, "KVKK metni bağlantısı"));
        }

        ///// <summary>
        ///// KVKK HTML içeriğini döner (PDF metninden parse edilmiştir) // yazılar düzeltilmeli
        ///// 
        ///// </summary>
        //[HttpGet("kvkk-link")]
        //public IActionResult GetDocumentHtml()
        //{
        //    var pdfPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs", "MSC Kruvaziyer_Katılımcı Aydınlatma Metni.pdf");

        //    if (!System.IO.File.Exists(pdfPath))
        //        return NotFound(new Result(false, "KVKK PDF dosyası bulunamadı."));

        //    var htmlContent = ConvertPdfToHtml(pdfPath);
        //    return Ok(new DataResult<string>(htmlContent, true, "KVKK metni HTML içeriği"));
        //}

        //private string ConvertPdfToHtml(string pdfPath)
        //{
        //    if (_cachedKvkkHtml != null)
        //        return _cachedKvkkHtml;

        //    var builder = new StringBuilder();
        //    builder.Append("<div class='kvkk-text'>");

        //    using var document = PdfDocument.Open(pdfPath);
        //    foreach (var page in document.GetPages())
        //    {
        //        builder.Append("<p>");
        //        builder.Append(System.Net.WebUtility.HtmlEncode(page.Text));
        //        builder.Append("</p>");
        //    }

        //    builder.Append("</div>");
        //    _cachedKvkkHtml = builder.ToString();
        //    return _cachedKvkkHtml;
        //}

    }

}
