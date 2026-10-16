using API_PRIMECRM.Domain.DTOs.Sales;
using API_PRIMECRM.Domain.Interfaces.Services;
using API_PRIMECRM.Domain.Models.Ventas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_PRIMECRM.Controllers.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        // ==========================
        // CREAR VENTA
        // ==========================

        [HttpPost]
        public async Task<ActionResult<Sale>> Create(
            [FromBody] CreateSaleDto dto)
        {
            try
            {
                var sale = await _saleService.CreateAsync(dto);

                return StatusCode(201, sale);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================
        // OBTENER TODAS LAS VENTAS
        // ==========================

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sale>>> GetAll()
        {
            var sales = await _saleService.GetAllAsync();

            return Ok(sales);
        }

        // ==========================
        // OBTENER VENTA POR ID
        // ==========================

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Sale>> GetById(int id)
        {
            try
            {
                var sale = await _saleService.GetByIdAsync(id);

                return Ok(sale);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // ==========================
        // VENTAS POR PRODUCTO
        // ==========================

        [HttpGet("product/{productId:int}")]
        public async Task<ActionResult<IEnumerable<Sale>>> GetByProductId(
            int productId)
        {
            var sales = await _saleService
                .GetByProductIdAsync(productId);

            return Ok(sales);
        }

        // ==========================
        // VENTAS POR ESTADO
        // ==========================

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Sale>>> GetByStatus(
            SaleStatus status)
        {
            var sales = await _saleService
                .GetByStatusAsync(status);

            return Ok(sales);
        }

        // ==========================
        // VENTAS POR RANGO DE FECHAS
        // ==========================

        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<Sale>>> GetByDateRange(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            if (!from.HasValue || !to.HasValue)
            {
                return BadRequest(new
                {
                    message = "Debe proporcionar ambas fechas."
                });
            }

            if (from.Value > to.Value)
            {
                return BadRequest(new
                {
                    message = "La fecha inicial no puede superar la final."
                });
            }

            var sales = await _saleService.GetByDateRangeAsync(
                from.Value,
                to.Value);

            return Ok(sales);
        }

        // ==========================
        // MARCAR COMO ENTREGADA
        // ==========================

        [HttpPatch("{id:int}/delivered")]
        public async Task<IActionResult> MarkAsDelivered(int id)
        {
            try
            {
                await _saleService.MarkAsDeliveredAsync(id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // ==========================
        // MARCAR COMO PERDIDA
        // ==========================

        [HttpPatch("{id:int}/lost")]
        public async Task<IActionResult> MarkAsLost(int id)
        {
            try
            {
                await _saleService.MarkAsLostAsync(id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // ==========================
        // MARCAR COMO NO ENTREGADA
        // ==========================

        [HttpPatch("{id:int}/not-delivered")]
        public async Task<IActionResult> MarkAsNotDelivered(int id)
        {
            try
            {
                await _saleService.MarkAsNotDeliveredAsync(id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // ==========================
        // ACTUALIZAR TRACKING
        // ==========================

        [HttpPatch("{id:int}/tracking")]
        public async Task<IActionResult> UpdateTracking(
            int id,
            [FromBody] UpdateSaleTrackingDto dto)
        {
            try
            {
                await _saleService.UpdateTrackingAsync(
                    id,
                    dto.TrackingNumber);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
