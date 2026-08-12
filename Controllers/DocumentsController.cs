using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;
using System.Security.Claims;
using Path = System.IO.Path;

namespace ProjectManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly DocumentRepository _documentRepository;
        private readonly IWebHostEnvironment _env;

        public DocumentsController(DocumentRepository documentRepository, IWebHostEnvironment env)
        {
            _documentRepository = documentRepository;
            _env = env;
        }

        // GET: api/documents
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var docs = await _documentRepository.GetAll();
            return Ok(docs);
        }

        // POST: api/documents  (multipart/form-data, field name "file")
        [HttpPost]
        [Authorize(Roles = "Admin,Project Manager")]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] int? projectId)
        {
            if (file == null || file.Length == 0) return BadRequest("No file provided.");

            var uploadsFolder = Path.Combine(_env.ContentRootPath, "Uploads");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueName = $"{Guid.NewGuid()}_{file.FileName}";
            var fullPath = Path.Combine(uploadsFolder, uniqueName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var doc = new DocumentModel
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                FileSizeBytes = file.Length,
                StoredPath = uniqueName,
                ProjectId = projectId,
                UploadedBy = userId,
            };

            var newId = await _documentRepository.Create(doc);
            return Ok(new { documentId = newId });
        }

        // GET: api/documents/5/download
        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(int id)
        {
            var doc = await _documentRepository.GetById(id);
            if (doc == null) return NotFound();

            var fullPath = Path.Combine(_env.ContentRootPath, "Uploads", doc.StoredPath);
            if (!System.IO.File.Exists(fullPath)) return NotFound("File missing on server.");

            var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            return File(bytes, doc.ContentType ?? "application/octet-stream", doc.FileName);
        }

        // DELETE: api/documents/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Project Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var doc = await _documentRepository.GetById(id);
            if (doc == null) return NotFound();

            var fullPath = Path.Combine(_env.ContentRootPath, "Uploads", doc.StoredPath);
            if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);

            await _documentRepository.Delete(id);
            return NoContent();
        }
    }
}