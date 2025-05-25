using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace UserService.Domain.Commands
{
    public class UploadProfilePhotoCommand : IRequest<string>
    {
        public int UserId { get; set; }
        public IFormFile Photo { get; set; }
    }
}
