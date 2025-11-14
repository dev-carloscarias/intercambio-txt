using com.InnovaMD.Provider.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace InnovaCare.HCSS.Services.Controllers
{
    [Authorize(Policy = "BearerAuthenticated")]
    [Route("api/[controller]")]
    public class DocumentController : Controller
    {
        private readonly IDocumentComponent documentComponent;

        public DocumentController(IDocumentComponent documentComponent)
        {
            this.documentComponent = documentComponent;
        }

        [AllowAnonymous]
        [HttpGet("{guid}/print")]
        public IActionResult GetDocumentCacheForPrint(Guid guid)
        {
            if (guid == default) return BadRequest();

            var reportModel = documentComponent.RetrieveDocumentCache(guid);

            if (reportModel == null) return NotFound();

            var userAgent = Request.Headers["User-Agent"].FirstOrDefault();
            if (userAgent != null && userAgent.Contains("Firefox"))
            {
                return File(reportModel.Content, reportModel.ContentType, reportModel.FileName);
            }

            Response.Headers.Add("Content-Disposition", "inline; filename=" + reportModel.FileName);
            return File(reportModel.Content, reportModel.ContentType);
        }

        [AllowAnonymous]
        [HttpGet("{guid}/save")]
        public IActionResult GetDocumentCache(Guid guid)
        {
            if (guid == default) return BadRequest();

            var reportModel = documentComponent.RetrieveDocumentCache(guid);

            if (reportModel == null) return NotFound();

            var fileContentResult = new FileContentResult(reportModel.Content, reportModel.ContentType)
            {
                FileDownloadName = reportModel.FileName
            };

            return fileContentResult;
        }
    }
}
