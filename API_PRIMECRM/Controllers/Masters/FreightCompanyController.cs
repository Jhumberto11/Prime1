using API_PRIMECRM.Application.Master;
using API_PRIMECRM.Domain.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_PRIMECRM.Controllers.Masters
{
    [Route("api/[controller]")]
    [ApiController]
    public class FreightCompanyController : ControllerBase
    {
        private readonly FreightCompanyAdminService _freightCompanyAdminService;
        public FreightCompanyController(FreightCompanyAdminService freightCompanyAdminService)
        {
            _freightCompanyAdminService = freightCompanyAdminService;
        }

        public async Task<IActionResult> GetAllFreightCompanies()
        {
            var freightCompanies = await _freightCompanyAdminService.GetAllFreightCompanyAsync();
            return Ok(freightCompanies);
        }

        public async Task<IActionResult> GetFreightCompanyById(int id)
        {
            try
            {
                var freightCompany = await _freightCompanyAdminService.GetFreightCompanyByIdAsync(id);
                return Ok(freightCompany);
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

        public async Task<IActionResult> CreateFreightCompanyAsync(FreightCompany freightCompany)
        {
            try
            {
                var newFreightCompany = await _freightCompanyAdminService.AddFreightCompanyAsync(freightCompany);
                return CreatedAtAction(nameof(GetFreightCompanyById), new { id = newFreightCompany.Id }, newFreightCompany);
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
        public async Task<IActionResult> UpdateFreightCompanyAsync(int id, FreightCompany freightCompany)
        {
            try
            {
                if (id != freightCompany.Id)
                {
                    return BadRequest(new
                    {
                        message = "El Id de la compañía de transporte no coincide con el Id proporcionado."
                    });
                }
                await _freightCompanyAdminService.UpdateFreightCompanyAsync(freightCompany);
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

        public async Task<IActionResult> DesactivateFreightCompanyAsync(int id)
        {
            try
            {
                await _freightCompanyAdminService.DesactivateFreightCompanyAsync(id);
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
        public async Task<IActionResult> ActivateFreightCompanyAsync(int id)
        {
            try
            {
                await _freightCompanyAdminService.ActivateFreightCompanyAsync(id);
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
