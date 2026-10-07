using API_PRIMECRM.Application.Services.Master;
using API_PRIMECRM.Domain.DTOs.Masters;
using API_PRIMECRM.Domain.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_PRIMECRM.Controllers.Masters
{
    [Route("api/sales-channels")]
    [ApiController]
    public class SaleChannelController : ControllerBase
    {
        private readonly SaleChannelProvider _saleChannelProvider;

        public SaleChannelController(
            SaleChannelProvider saleChannelProvider)
        {
            _saleChannelProvider = saleChannelProvider;
        }


        // GET: api/sales-channels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesChannel>>>
            GetAllSaleChannels()
        {
            var channels =
                await _saleChannelProvider.GetAllSaleChannelsAsync();

            return Ok(channels);
        }


        // GET: api/sales-channels/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SalesChannel>>
            GetSaleChannelById(int id)
        {
            try
            {
                var channel =
                    await _saleChannelProvider.GetSaleChannelAsync(id);

                return Ok(channel);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }


        // POST: api/sales-channels
        [HttpPost]
        public async Task<ActionResult<SalesChannel>>
            CreateSaleChannel([FromBody] SaleChannelDto dto)
        {
            try
            {
                var channel = new SalesChannel
                {
                    Name = dto.Name
                };

                var createdChannel =
                    await _saleChannelProvider
                        .AddSaleChannelAsync(channel);

                return CreatedAtAction(
                    nameof(GetSaleChannelById),
                    new { id = createdChannel.Id },
                    createdChannel
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }


        // PUT: api/sales-channels/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<SalesChannel>>
            UpdateSaleChannel(
                int id,
                [FromBody] SaleChannelDto dto)
        {
            try
            {
                var channel = new SalesChannel
                {
                    Id = id,
                    Name = dto.Name
                };

                var updatedChannel =
                    await _saleChannelProvider
                        .UpdateSaleChannelAsync(channel);

                return Ok(updatedChannel);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }


        // PATCH: api/sales-channels/5/deactivate
        [HttpPatch("{id:int}/desactivate")]
        public async Task<IActionResult>
            DeactivateSaleChannel(int id)
        {
            try
            {
                await _saleChannelProvider
                    .DeactivateSaleChannelAsync(id);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }


        // PATCH: api/sales-channels/5/activate
        [HttpPatch("{id:int}/activate")]
        public async Task<IActionResult>
            ActivateSaleChannel(int id)
        {
            try
            {
                await _saleChannelProvider
                    .ActivateSaleChannelAsync(id);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
    }
}
