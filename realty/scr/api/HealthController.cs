using Microsoft.AspNetCore.Mvc;

namespace ServiceDesk.Api.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HealthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Проверка работоспособности API и JSON-хранилища
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            var equipmentFile = _configuration["Storage:EquipmentFile"] 
                                ?? "Data/equipment.json";
            var requestsFile = _configuration["Storage:ServiceRequestsFile"] 
                                ?? "Data/serviceRequests.json";

            try
            {
                CheckFile(equipmentFile);
                CheckFile(requestsFile);

                return Ok(new
                {
                    status = "Healthy",
                    storage = "JSON",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    status = "Unhealthy",
                    storage = "JSON",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        private static void CheckFile(string path)
        {
            var fullPath = Path.GetFullPath(path);
            var dir = Path.GetDirectoryName(fullPath);

            if (string.IsNullOrWhiteSpace(dir))
                throw new InvalidOperationException("Invalid storage path");

            // Создаём папку, если нет
            Directory.CreateDirectory(dir);

            // Если файла нет — создаём пустой массив
            if (!System.IO.File.Exists(fullPath))
            {
                System.IO.File.WriteAllText(fullPath, "[]");
            }

            // Проверяем, что файл можно прочитать
            System.IO.File.ReadAllText(fullPath);
        }
    }
}
