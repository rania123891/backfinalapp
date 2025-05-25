// UserService.Application/Handlers/UpdateProfilePictureHandler.cs
using MediatR;
using UserService.Domain.Commands;
using UserService.Domain.Interfaces;
using UserService.Domain.Interfaces.Infrastructure;
public class UploadProfilePhotoHandler : IRequestHandler<UploadProfilePhotoCommand, string>
{
    private readonly IUtilisateurRepository _userRepository;
    private readonly IFileStorageService _fileStorage;

    public UploadProfilePhotoHandler(IUtilisateurRepository userRepository, IFileStorageService fileStorage)
    {
        _userRepository = userRepository;
        _fileStorage = fileStorage;
    }

    public async Task<string> Handle(UploadProfilePhotoCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
            throw new Exception("Utilisateur non trouvé");

        var photoUrl = await _fileStorage.SaveFileAsync(request.Photo, "profile-photos");
        user.ProfilePhotoUrl = photoUrl;
        await _userRepository.UpdateAsync(user);

        return photoUrl;
    }
}