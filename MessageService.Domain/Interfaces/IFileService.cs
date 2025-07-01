using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MessageService.Domain.Models;

namespace MessageService.Domain.Interfaces
{
    public interface IFileService
    {
        // Méthodes pour FileController (gestion de fichiers avec IDs)
        Task<FileUpload> UploadFileAsync(IFormFile file, string expediteurId, string destinataireId);
        Task<(byte[] fileBytes, string contentType, string fileName)> DownloadFileAsync(string fileId);
        Task<(byte[] fileBytes, string contentType)> GetFilePreviewAsync(string fileId);
        Task<bool> DeleteFileAsync(string fileId);

        // Méthodes pour MessagesController et AttachmentsController (gestion par path)
        Task<string> SaveFileAsync(IFormFile file, string directory);
        Task<byte[]> GetFileAsync(string filePath);
        Task DeleteFileByPathAsync(string filePath);

        // Méthodes utilitaires
        string GetFileType(string contentType);
        bool IsValidFileType(string contentType);
        bool IsValidFileSize(long fileLength);
    }
}