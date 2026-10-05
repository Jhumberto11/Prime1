using API_PRIMECRM.Application.Services.Master;
using API_PRIMECRM.Domain.DTOs.Masters;
using API_PRIMECRM.Domain.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_PRIMECRM.Controllers.Masters
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouriersController : ControllerBase
    {
        private readonly CourierCompanyAdminService _courierCompanyAdminService;
        public CouriersController(CourierCompanyAdminService courierCompanyAdminService)
        {
            _courierCompanyAdminService = courierCompanyAdminService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCouriers()
        {
            var couriers = await _courierCompanyAdminService.GetAllCourierCompanyAsync();
            return Ok(couriers);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourierById(int id)
        {
            try
            {
                var courier = await _courierCompanyAdminService.GetCourierCompanyByIdAsync(id);
                return Ok(courier);
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

        [HttpPost]
        public async Task<IActionResult> CreateCourierAsync(CourierDto dto)
        {
            CourierCompany newCourier = new CourierCompany
            {
                Name = dto.Name,
                DeliveryRate = dto.DeliveryRate,
                CashHandlingValue = dto.CashHandlingValue
            };
            try
            {
                var newCourieCompany = await _courierCompanyAdminService.AddCourierCompanyAsync(newCourier);
                return CreatedAtAction(nameof(GetCourierById), new { id = newCourieCompany.Id }, newCourieCompany);
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourierAsync(int id, CourierDto updateDto)
        {

            CourierCompany courier = await _courierCompanyAdminService.GetCourierCompanyByIdAsync(id);
            if(courier == null)
            {
                return BadRequest(new
                {
                    message = "La compañía de transporte no fue encontrada."
                });
            }
            try
            {
                if (id != courier.Id)
                {
                    return BadRequest(new
                    {
                        message = "El Id proporcionado no coincide con el Id de la compañía de transporte."
                    });
                }

                /// Actualizacion de Campos
                courier.Name = updateDto.Name;
                courier.DeliveryRate = updateDto.DeliveryRate;
                courier.CashHandlingValue = updateDto.CashHandlingValue;



                await _courierCompanyAdminService.UpdateCourierCompanyAsync(courier);
                return Ok(new
                {
                    message = "Compañía de transporte actualizada correctamente."
                });
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

        [HttpDelete("desactivate/{id}")]
        public async Task<IActionResult> DesactivateCourierAsync(int id)
        {

            try
            {
                await _courierCompanyAdminService.DesactivateCourierCompanyAsync(id);
                return Ok(new
                {
                    message = "Compañía de transporte desactivada correctamente."
                });
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

        [HttpPatch("activate/{id}")]
        public async Task<IActionResult> ActivateCourierAsync(int id)
        {

            try
            {
                await _courierCompanyAdminService.ActivateCourierCompanyAsync(id);
                return Ok(new
                {
                    message = "Compañía de transporte activada correctamente."
                });
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
