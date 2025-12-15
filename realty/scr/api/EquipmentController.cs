using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Api.Models;
using ServiceDesk.Api.Repositories;

namespace ServiceDesk.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentRepository _repository;

        public EquipmentController(IEquipmentRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Получить список всего оборудования
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Equipment>>> GetAll()
        {
            var equipment = await _repository.GetAllAsync();
            return Ok(equipment);
        }

        /// <summary>
        /// Получить оборудование по идентификатору
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Equipment>> GetById(int id)
        {
            var equipment = await _repository.GetByIdAsync(id);
            if (equipment == null)
                return NotFound();

            return Ok(equipment);
        }

        /// <summary>
        /// Создать новое оборудование
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Equipment>> Create([FromBody] Equipment equipment)
        {
            if (equipment == null)
                return BadRequest();

            var created = await _repository.CreateAsync(equipment);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        /// <summary>
        /// Обновить существующее оборудование
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Equipment equipment)
        {
            if (equipment == null)
                return BadRequest();

            var updated = await _repository.UpdateAsync(id, equipment);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Удалить оборудование
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
