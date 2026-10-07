using API_PRIMECRM.Application.Services.Master;
using API_PRIMECRM.Domain.DTOs.Masters;
using API_PRIMECRM.Domain.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_PRIMECRM.Controllers.Masters
{
    [Route("api/payment-methods")]
    [ApiController]
    public class PaymentMethodController : ControllerBase
    {
        private readonly PaymentMethodProvider _paymentMethodProvider;

        public PaymentMethodController(
            PaymentMethodProvider paymentMethodProvider)
        {
            _paymentMethodProvider = paymentMethodProvider;
        }


        // GET: api/payment-methods
        [HttpGet]
        [ProducesResponseType(
            typeof(IEnumerable<PaymentMethod>),
            StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PaymentMethod>>>
            GetAllPaymentMethods()
        {
            var paymentMethods =
                await _paymentMethodProvider
                    .GetAllPaymentMethodsAsync();

            return Ok(paymentMethods);
        }


        // GET: api/payment-methods/5
        [HttpGet("{id:int}")]
        [ProducesResponseType(
            typeof(PaymentMethod),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentMethod>>
            GetPaymentMethodById(int id)
        {
            try
            {
                var paymentMethod =
                    await _paymentMethodProvider
                        .GetPaymentMethodByIdAsync(id);

                return Ok(paymentMethod);
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


        // POST: api/payment-methods
        [HttpPost]
        [ProducesResponseType(
            typeof(PaymentMethod),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PaymentMethod>>
            CreatePaymentMethod(
                [FromBody] PaymentMethodDto dto)
        {
            try
            {
                var paymentMethod = new PaymentMethod
                {
                    Name = dto.Name,
                    Type = dto.Type,
                    BankName = dto.BankName,
                    Last4 = dto.Last4,
                    IsActive = true
                };

                var createdPaymentMethod =
                    await _paymentMethodProvider
                        .AddPaymentMethodAsync(paymentMethod);

                return CreatedAtAction(
                    nameof(GetPaymentMethodById),
                    new { id = createdPaymentMethod.Id },
                    createdPaymentMethod);
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


        // PUT: api/payment-methods/5
        [HttpPut("{id:int}")]
        [ProducesResponseType(
            typeof(PaymentMethod),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PaymentMethod>>
            UpdatePaymentMethod(
                int id,
                [FromBody] PaymentMethodDto dto)
        {
            try
            {
                var paymentMethod = new PaymentMethod
                {
                    Id = id,
                    Name = dto.Name,
                    Type = dto.Type,
                    BankName = dto.BankName,
                    Last4 = dto.Last4
                };

                var updatedPaymentMethod =
                    await _paymentMethodProvider
                        .UpdatePaymentMethodAsync(paymentMethod);

                return Ok(updatedPaymentMethod);
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


        // PATCH: api/payment-methods/5/deactivate
        [HttpPatch("{id:int}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult>
            DeactivatePaymentMethod(int id)
        {
            try
            {
                await _paymentMethodProvider
                    .DeactivatePaymentMethodAsync(id);

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


        // PATCH: api/payment-methods/5/activate
        [HttpPatch("{id:int}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult>
            ActivatePaymentMethod(int id)
        {
            try
            {
                await _paymentMethodProvider
                    .ActivatePaymentMethodAsync(id);

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
