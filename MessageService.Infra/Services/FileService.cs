using MessageService.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MessageService.Domain.Models;
using Microsoft.AspNetCore.StaticFiles;

namespace MessageService.Infra.Services
{
    public class FileService : IFileService
    {
        private readonly IHostEnvironment _environment;
        private readonly ILogger<FileService> _logger;
        private readonly string _uploadsPath;

        private readonly string[] _allowedExtensions = new[]
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", // Images
            ".pdf", // PDF
            ".doc", ".docx", // Word
            ".xls", ".xlsx", // Excel
            ".txt", // Texte
            ".zip", ".rar" // Archives
        };

        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

        public FileService(IHostEnvironment environment, ILogger<FileService> logger)
        {
            _environment = environment;
            _logger = logger;
            _uploadsPath = Path.Combine(_environment.ContentRootPath, "uploads");

            // Créer le dossier uploads s'il n'existe pas
            if (!Directory.Exists(_uploadsPath))
            {
                Directory.CreateDirectory(_uploadsPath);
                _logger.LogInformation($"Dossier uploads créé : {_uploadsPath}");
            }
        }

        // Méthodes pour FileController (gestion avec IDs)
        public async Task<FileUpload> UploadFileAsync(IFormFile file, string expediteurId, string destinataireId)
        {
            if (!IsValidFileSize(file.Length))
            {
                throw new ArgumentException($"Le fichier est trop volumineux (max: {MaxFileSize / (1024 * 1024)}MB)");
            }

            if (!IsValidFileType(file.ContentType))
            {
                var fileExtension = Path.GetExtension(file.FileName);
                throw new ArgumentException($"Type de fichier non autorisé: {fileExtension}");
            }

            // Générer un nom de fichier unique
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_uploadsPath, uniqueFileName);

            // Sauvegarder le fichier
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation($"Fichier uploadé: {file.FileName} -> {uniqueFileName}");

            // Créer l'entité FileUpload
            var fileUpload = new FileUpload
            {
                Id = uniqueFileName,
                OriginalName = file.FileName,
                StoredName = uniqueFileName,
                MimeType = file.ContentType ?? "application/octet-stream",
                Size = file.Length,
                UploadDate = DateTime.UtcNow,
                UploadedBy = expediteurId,
                FilePath = filePath
            };

            return fileUpload;
        }

        public async Task<(byte[] fileBytes, string contentType, string fileName)> DownloadFileAsync(string fileId)
        {
            var filePath = Path.Combine(_uploadsPath, fileId);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Fichier non trouvé");
            }

            var fileBytes = await File.ReadAllBytesAsync(filePath);

            // Déterminer le type de contenu
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return (fileBytes, contentType, fileId);
        }

        public async Task<(byte[] fileBytes, string contentType)> GetFilePreviewAsync(string fileId)
        {
            var filePath = Path.Combine(_uploadsPath, fileId);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Fichier non trouvé");
            }

            var fileBytes = await File.ReadAllBytesAsync(filePath);

            // Déterminer le type de contenu
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return (fileBytes, contentType);
        }

        public async Task<bool> DeleteFileAsync(string fileId)
        {
            var filePath = Path.Combine(_uploadsPath, fileId);

            if (!File.Exists(filePath))
            {
                return false;
            }

            try
            {
                File.Delete(filePath);
                _logger.LogInformation($"Fichier supprimé: {fileId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la suppression du fichier {fileId}");
                throw;
            }
        }

        // Méthodes pour MessagesController et AttachmentsController (gestion par path)
        public async Task<string> SaveFileAsync(IFormFile file, string directory)
        {
            if (!IsValidFileSize(file.Length))
            {
                throw new ArgumentException($"Le fichier est trop volumineux (max: {MaxFileSize / (1024 * 1024)}MB)");
            }

            if (!IsValidFileType(file.ContentType))
            {
                throw new ArgumentException($"Type de fichier non autorisé: {file.ContentType}");
            }

            // Créer le dossier de destination s'il n'existe pas
            var targetDirectory = Path.Combine(_environment.ContentRootPath, directory);
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // Générer un nom de fichier unique
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(targetDirectory, uniqueFileName);

            // Sauvegarder le fichier
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation($"Fichier sauvegardé: {file.FileName} -> {filePath}");

            return filePath;
        }

        public async Task<byte[]> GetFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Fichier non trouvé");
            }

            return await File.ReadAllBytesAsync(filePath);
        }

        public async Task DeleteFileByPathAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation($"Fichier supprimé: {filePath}");
            }

            await Task.CompletedTask;
        }

        // Méthodes utilitaires
        public string GetFileType(string contentType)
        {
            return contentType switch
            {
                "image/jpeg" or "image/jpg" => "Image",
                "image/png" => "Image",
                "image/gif" => "Image",
                "image/webp" => "Image",
                "application/pdf" => "PDF",
                "application/msword" or "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => "Document",
                "application/vnd.ms-excel" or "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => "Spreadsheet",
                "text/plain" => "Text",
                "application/zip" or "application/x-rar-compressed" => "Archive",
                _ => "Autre"
            };
        }

        public bool IsValidFileType(string contentType)
        {
            var validTypes = new[]
            {
                "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp",
                "application/pdf",
                "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "text/plain",
                "application/zip", "application/x-rar-compressed"
            };

            return Array.Exists(validTypes, type => type.Equals(contentType, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsValidFileSize(long fileLength)
        {
            return fileLength <= MaxFileSize && fileLength > 0;
        }
    }
}