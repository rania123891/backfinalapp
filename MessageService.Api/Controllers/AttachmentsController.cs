using Microsoft.AspNetCore.Mvc;
using MessageService.Data.Repositories;
using MessageService.Domain.Interfaces;

namespace MessageService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttachmentsController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<AttachmentsController> _logger;

        public AttachmentsController(
            IFileService fileService,
            IMessageRepository messageRepository,
            ILogger<AttachmentsController> logger)
        {
            _fileService = fileService;
            _messageRepository = messageRepository;
            _logger = logger;
        }

        [HttpGet("{attachmentId}/download")]
        public async Task<IActionResult> DownloadAttachment(Guid attachmentId)
        {
            try
            {
                var attachment = await _messageRepository.GetAttachmentByIdAsync(attachmentId);
                if (attachment == null)
                {
                    return NotFound("Pièce jointe non trouvée");
                }

                var fileBytes = await _fileService.GetFileAsync(attachment.FilePath);
                return File(fileBytes, attachment.MimeType, attachment.OriginalFileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound("Fichier non trouvé");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors du téléchargement de la pièce jointe {attachmentId}: {ex.Message}");
                return StatusCode(500, "Erreur lors du téléchargement");
            }
        }

        [HttpGet("{attachmentId}/preview")]
        public async Task<IActionResult> PreviewAttachment(Guid attachmentId)
        {
            try
            {
                var attachment = await _messageRepository.GetAttachmentByIdAsync(attachmentId);
                if (attachment == null)
                {
                    return NotFound("Pièce jointe non trouvée");
                }

                var fileBytes = await _fileService.GetFileAsync(attachment.FilePath);
                return File(fileBytes, attachment.MimeType);
            }
            catch (FileNotFoundException)
            {
                return NotFound("Fichier non trouvé");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la prévisualisation de la pièce jointe {attachmentId}: {ex.Message}");
                return StatusCode(500, "Erreur lors de la prévisualisation");
            }
        }

        [HttpDelete("{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment(Guid attachmentId)
        {
            try
            {
                var attachment = await _messageRepository.GetAttachmentByIdAsync(attachmentId);
                if (attachment == null)
                {
                    return NotFound("Pièce jointe non trouvée");
                }

                // Supprimer le fichier physique
                await _fileService.DeleteFileByPathAsync(attachment.FilePath);

                // Supprimer l'enregistrement en base de données
                await _messageRepository.DeleteAttachmentAsync(attachmentId);

                return Ok(new { message = "Pièce jointe supprimée avec succès" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la suppression de la pièce jointe {attachmentId}: {ex.Message}");
                return StatusCode(500, "Erreur lors de la suppression");
            }
        }

        [HttpGet("{attachmentId}/info")]
        public async Task<IActionResult> GetAttachmentInfo(Guid attachmentId)
        {
            try
            {
                var attachment = await _messageRepository.GetAttachmentByIdAsync(attachmentId);
                if (attachment == null)
                {
                    return NotFound("Pièce jointe non trouvée");
                }

                var info = new
                {
                    id = attachment.Id,
                    fileName = attachment.FileName,
                    originalFileName = attachment.OriginalFileName,
                    fileType = attachment.FileType,
                    mimeType = attachment.MimeType,
                    fileSize = attachment.FileSize,
                    uploadedAt = attachment.UploadedAt,
                    uploadedBy = attachment.UploadedBy,
                    messageId = attachment.MessageId
                };

                return Ok(info);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération des informations de la pièce jointe {attachmentId}: {ex.Message}");
                return StatusCode(500, "Erreur lors de la récupération des informations");
            }
        }
    }
}