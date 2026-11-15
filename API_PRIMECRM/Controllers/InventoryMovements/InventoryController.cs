using API_PRIMECRM.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API_PRIMECRM.Controllers.InventoryMovements
{
    [Route("api/[controller]")]
    [ApiController]


    public class InventoryController : ControllerBase
    {

        private readonly IInventoryMovementsService _inventoryMovementsService;
        public InventoryController(IInventoryMovementsService inventoryMovementsService)
        { 
            _inventoryMovementsService = inventoryMovementsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTotalInventory()
        {
            var inventory = await _inventoryMovementsService.GetActualInventory();

            return Ok(inventory);
        }
    }
}
