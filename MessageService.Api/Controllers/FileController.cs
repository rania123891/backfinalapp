using Microsoft.AspNetCore.Mvc;
using MessageService.Domain.Interfaces;
using MessageService.Domain.Models;

namespace MessageService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FileController> _logger;

        public FileController(IFileService fileService, ILogger<FileController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(
            IFormFile file,
            [FromForm] string expediteurId,
            [FromForm] string destinataireId)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "Aucun fichier fourni" });
                }

                if (!_fileService.IsValidFileSize(file.Length))
                {
                    return BadRequest(new { error = "Fichier trop volumineux (max: 10MB)" });
                }

                if (!_fileService.IsValidFileType(file.FileName))
                {
                    var extension = Path.GetExtension(file.FileName);
                    return BadRequest(new { error = $"Type de fichier non autorisé: {extension}" });
                }

                var fileUpload = await _fileService.UploadFileAsync(file, expediteurId, destinataireId);

                var response = new
                {
                    id = fileUpload.Id,
                    filename = fileUpload.StoredName,
                    originalName = fileUpload.OriginalName,
                    size = fileUpload.Size,
                    mimeType = fileUpload.MimeType,
                    uploadDate = fileUpload.UploadDate,
                    uploadedBy = fileUpload.UploadedBy
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erreur de validation lors de l'upload");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'upload du fichier");
                return StatusCode(500, new { error = "Erreur interne du serveur" });
            }
        }

        [HttpGet("download/{fileId}")]
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            try
            {
                var (fileBytes, contentType, fileName) = await _fileService.DownloadFileAsync(fileId);
                return File(fileBytes, contentType, fileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound(new { error = "Fichier non trouvé" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors du téléchargement du fichier {fileId}");
                return StatusCode(500, new { error = "Erreur lors du téléchargement" });
            }
        }

        [HttpGet("preview/{fileId}")]
        public async Task<IActionResult> PreviewFile(string fileId)
        {
            try
            {
                var (fileBytes, contentType) = await _fileService.GetFilePreviewAsync(fileId);
                return File(fileBytes, contentType);
            }
            catch (FileNotFoundException)
            {
                return NotFound(new { error = "Fichier non trouvé" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la prévisualisation du fichier {fileId}");
                return StatusCode(500, new { error = "Erreur lors de la prévisualisation" });
            }
        }

       

        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            return Ok(new
            {
                maxFileSize = 10 * 1024 * 1024,
                allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".zip", ".rar" },
                message = "Service de fichiers fonctionnel",
                architecture = "Clean Architecture",
                layers = new[] { "Api", "Domain", "Data", "Infra" }
            });
        }
    }
}