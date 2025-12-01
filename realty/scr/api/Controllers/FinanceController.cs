using Microsoft.AspNetCore.Mvc;
using Economic_bottom.Core.Services;
using Economic_bottom.API.Models.Requests;
using Economic_bottom.API.Models.Responses;

namespace Economic_bottom.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinanceController : ControllerBase
    {
        private readonly FinanceService _service;

        public FinanceController(FinanceService service)
        {
            _service = service;
        }

        // -----------------------------
        // 1. TCO CALCULATION
        // -----------------------------
        [HttpPost("tco/calc")]
        public IActionResult CalculateTco([FromBody] TcoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _service.CalculateTco(
                request.InitialCost,
                request.OperatingCost,
                request.MaintenanceCost,
                request.LifetimeYears
            );

            return Ok(new TcoResponse { TotalTco = result });
        }

        // -----------------------------
        // 2. ROI CALCULATION
        // -----------------------------
        [HttpPost("roi/calc")]
        public IActionResult CalculateRoi([FromBody] RoiRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _service.CalculateRoi(
                request.Profit,
                request.Investment
            );

            return Ok(new RoiResponse { RoiPercent = result });
        }

        // -----------------------------
        // 3. PAYBACK PERIOD
        // -----------------------------
        [HttpPost("payback/calc")]
        public IActionResult CalculatePayback([FromBody] PaybackRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _service.CalculatePayback(
                request.Investment,
                request.AnnualIncome
            );

            return Ok(new PaybackResponse { Years = result });
        }

        // -----------------------------
        // 4. SENSITIVITY ANALYSIS ±20%
        // -----------------------------
        [HttpPost("sensitivity/calc")]
        public IActionResult Sensitivity([FromBody] SensitivityRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _service.AnalyzeSensitivity(
                request.BaseValue,
                request.ChangePercentage // например 20 (%)
            );

            return Ok(result);
        }
    }
}
