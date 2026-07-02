using HealthCare.Claims.Monolith.Models;
using HealthCare.Claims.Monolith.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Claims.Monolith.Controllers;

[ApiController]
[Route("api/documents")]
public sealed class DocumentsController(DocumentService documents) : ControllerBase
{
    [HttpGet]
    public IActionResult GetDocuments() => Ok(documents.GetDocuments());

    [HttpPost]
    public IActionResult UploadDocument(UploadDocumentRequest request)
    {
        try
        {
            return Ok(documents.UploadDocument(request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{documentId}/verify")]
    public IActionResult VerifyDocument(string documentId, VerifyDocumentRequest request)
    {
        try
        {
            return Ok(documents.VerifyDocument(documentId, request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
