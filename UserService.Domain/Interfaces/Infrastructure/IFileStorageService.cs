// UserService.Domain/Interfaces/Infrastructure/IFileStorageService.cs
using Microsoft.AspNetCore.Http;

namespace UserService.Domain.Interfaces.Infrastructure;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string folder);
}