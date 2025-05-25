using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.DTOs
{
    public class CreateMessageDto
    {
        public string Contenu { get; set; }
        public string ExpediteurEmail { get; set; }
        public string EmailDestinataire { get; set; }
    }


}
