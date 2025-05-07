using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjetService.Domain.Interfaces;
using ProjetService.Domain.Commands;
using ProjetService.Domain.Queries;

namespace ProjetService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T> : ControllerBase where T : class
    {
        private readonly IMediator _mediator;
        private readonly IGenericRepository<T> _repository;

        public GenericController(IMediator mediator, IGenericRepository<T> repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        // ✅ GET ALL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T>>> GetAll()
        {
            var entities = await _repository.GetAllAsync();
            return Ok(entities);
        }

        // ✅ GET BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<T>> GetById(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return NotFound($"L'entité avec l'ID {id} n'a pas été trouvée.");

            return Ok(entity);
        }

        // ✅ CREATE
        [HttpPost]
        public async Task<ActionResult<T>> Create([FromBody] T entity)
        {
            if (entity == null)
                return BadRequest("L'entité ne peut pas être vide.");

            await _repository.AddAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.GetHashCode() }, entity);
        }

        // ✅ UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] T entity)
        {
            if (entity == null)
                return BadRequest("L'entité ne peut pas être vide.");

            await _repository.UpdateAsync(entity);
            return NoContent();
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return NotFound($"L'entité avec l'ID {id} n'existe pas.");

            await _repository.DeleteAsync(entity);
            return NoContent();
        }
    }
}
