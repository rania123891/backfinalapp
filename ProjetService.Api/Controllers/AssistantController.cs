using Microsoft.AspNetCore.Mvc;
using ProjetService.Domain.Interface;
using System.Threading.Tasks;

namespace ProjetService.Api.Controllers
{
    [ApiController]
    [Route("api/assistant")]
    public class AssistantController : ControllerBase
    {
        private readonly ICommandProcessor _commandProcessor;

        public AssistantController(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        [HttpPost("command")]
        public async Task<IActionResult> ProcessCommand([FromBody] CommandRequest request)
        {
            var result = await _commandProcessor.ProcessAsync(request.Command);
            return Ok(result);
        }
    }

    public class CommandRequest
    {
        public string Command { get; set; }
    }
}
