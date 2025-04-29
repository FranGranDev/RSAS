using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs.Test;
using Application.Services.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    [Authorize(Policy = "RequireManagerRole")]
    [ApiController]
    [Route("api/[controller]")]
    public class TestDataController : ControllerBase
    {
        private readonly ITestDataService _testDataService;

        public TestDataController(ITestDataService testDataService)
        {
            _testDataService = testDataService;
        }

        /// <summary>
        /// Генерирует тестовые продажи в указанный период
        /// </summary>
        /// <param name="dto">Параметры генерации тестовых данных</param>
        /// <returns>Результат операции</returns>
        [HttpPost("generate-sales")]
        public async Task<IActionResult> GenerateTestSales([FromBody] GenerateSalesDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { message = "Не удалось определить пользователя" });
                }

                await _testDataService.GenerateTestSalesAsync(dto, userId);
                return Ok(new { message = "Тестовые продажи успешно сгенерированы" });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 