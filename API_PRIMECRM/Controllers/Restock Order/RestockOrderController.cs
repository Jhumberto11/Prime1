using API_PRIMECRM.Application.Services.Restock_Orders;
using API_PRIMECRM.Domain.DTOs;
using API_PRIMECRM.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_PRIMECRM.Controllers.Restock_Order
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestockOrderController : ControllerBase
    {
        private readonly IRestockOrderService _restockOrderService;
        public RestockOrderController(IRestockOrderService restockOrderService)
        {
            _restockOrderService = restockOrderService;
        }

        public async Task<IActionResult> GetAll()
        {
            var orders = await _restockOrderService.GetAllAsync();
            return Ok(orders);
        }

        public async Task<IActionResult> GetById(int id)
        {
            var order = await _restockOrderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();
            return Ok(order);
        }
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var orders = await _restockOrderService.GetByProductIdAsync(productId);
            return Ok(orders);
        }

        public async Task<IActionResult> Create(CreateRestockOrderDto dto)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }
                var order = await _restockOrderService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

        }

        public async Task<IActionResult> RegisterActualCosts(int id, decimal otherCharges)
        {
            try
            {
                var order = await _restockOrderService.RegisterActualCostsAsync(id, otherCharges);
                if (order == null)
                    return NotFound();
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> Update(int id, UpdateRestockOrderDto dto)
        {
            try
            {
                var order = await _restockOrderService.UpdateAsync(id, dto);
                if (order == null)
                    return NotFound();
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }


        }
    }
}
