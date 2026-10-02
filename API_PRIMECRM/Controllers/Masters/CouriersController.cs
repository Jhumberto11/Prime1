using API_PRIMECRM.Application.Master;
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


        public async Task<IActionResult> GetAllCouriers()
        {
            var couriers = await _courierCompanyAdminService.GetAllCourierCompanyAsync();
            return Ok(couriers);
        }

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

        public async Task<IActionResult> CreateCourierAsync(CourierCompany courierCompany)
        {
            try
            {
                var newCourieCompany = await _courierCompanyAdminService.AddCourierCompanyAsync(courierCompany);
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
        public async Task<IActionResult> UpdateCourierAsync(int id, CourierCompany courierCompany)
        {
            try
            {
                if (id != courierCompany.Id)
                {
                    return BadRequest(new
                    {
                        message = "El Id proporcionado no coincide con el Id de la compañía de transporte."
                    });
                }
                await _courierCompanyAdminService.UpdateCourierCompanyAsync(courierCompany);
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
