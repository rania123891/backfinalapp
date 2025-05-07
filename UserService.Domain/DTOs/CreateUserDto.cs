using UserService.Domain.Models;

namespace UserService.Domain.DTOs
{
    public class CreateUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public RoleUtilisateur Role { get; set; }
    }
}
