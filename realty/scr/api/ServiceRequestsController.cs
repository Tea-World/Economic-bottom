using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Api.Models;
using ServiceDesk.Api.Repositories;

namespace ServiceDesk.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestRepository _requests;
        private readonly IEquipmentRepository _equipment;

        public ServiceRequestsController(
            IServiceRequestRepository requests,
            IEquipmentRepository equipment)
        {
            _requests = requests;
            _equipment = equipment;
        }

        /// <summary>
        /// Получить все заявки
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ServiceRequest>>> GetAll()
        {
            var items = await _requests.GetAllAsync();
            return Ok(items);
        }

        /// <summary>
        /// Получить заявку по id
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ServiceRequest>> GetById(int id)
        {
            var item = await _requests.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        /// <summary>
        /// Создать заявку
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ServiceRequest>> Create([FromBody] ServiceRequest request)
        {
            if (request == null)
                return BadRequest();

            // Обязательная проверка связи: equipmentId должен существовать
            var eq = await _equipment.GetByIdAsync(request.EquipmentId);
            if (eq == null)
            {
                return BadRequest(new
                {
                    error = "Invalid equipmentId",
                    message = $"Оборудование с id={request.EquipmentId} не найдено."
                });
            }

            // Рекомендуемое поведение сервера: выставить CreatedAt на сервере
            // (если ваш JsonServiceRequestRepository уже это делает — дубль не критичен,
            // но лучше выбрать одно место ответственности)
            if (request.CreatedAt == default)
                request.CreatedAt = DateTime.UtcNow;

            // Если requestNumber пустой — можно оставить как есть,
            // либо генерировать здесь (если вы решите).
            var created = await _requests.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        /// <summary>
        /// Обновить заявку
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServiceRequest request)
        {
            if (request == null)
                return BadRequest();

            // Если меняют equipmentId — проверяем, что оборудование существует
            var eq = await _equipment.GetByIdAsync(request.EquipmentId);
            if (eq == null)
            {
                return BadRequest(new
                {
                    error = "Invalid equipmentId",
                    message = $"Оборудование с id={request.EquipmentId} не найдено."
                });
            }

            var updated = await _requests.UpdateAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Удалить заявку
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _requests.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
