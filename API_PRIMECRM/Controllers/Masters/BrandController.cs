using API_PRIMECRM.Application.Services.Master;
using API_PRIMECRM.Domain.DTOs.Masters;
using API_PRIMECRM.Domain.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_PRIMECRM.Controllers.Masters
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {

        private readonly BrandService _brandService;

        public BrandController(BrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            if (brands == null || !brands.Any())
            {
                return NotFound(new
                {
                    message = "No se encontraron marcas registradas."
                });
            }
            return Ok(brands);
        }

        [HttpGet("activeBrands")]
        public async Task<IActionResult> GetActiveBrands()
        {
            var brands = await _brandService.GetActiveBrandsAsync();
            if (brands == null || !brands.Any())
            {
                return NotFound(new
                {
                    message = "No se encontraron marcas registradas."
                });
            }
            return Ok(brands);
        }

        // GET: api/brands/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(int id)
        {
            try
            {
                var brand = await _brandService.GetBrandByIdAsync(id);

                return Ok(brand);
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
        // POST: api/brands
        [HttpPost]
        public async Task<IActionResult> CreateBrandAsync(BrandDto brand)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(new
                {
                    message = "Datos de marca inválidos."
                });
            }
            try
            {
                Brand newBrand = new Brand
                {
                    Name = brand.Name,
                   
                };
                var createdBrand = await _brandService.AddBrandAsync(newBrand);

                return CreatedAtAction(
                    nameof(GetBrandById),
                    new { id = createdBrand.Id },
                    createdBrand
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

        // PUT: api/brands/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrandAsync(
            int id,
            BrandDto dto)
        {
            try
            {
                Brand brand = await _brandService.GetBrandByIdAsync(id) ?? throw new KeyNotFoundException("Marca no encontrada.");
                if (brand == null)
                {
                    return NotFound(new
                    {
                        message = "Marca no encontrada."
                    });
                }

                if (id != brand.Id)
                {
                    return BadRequest(new
                    {
                        message = "El Id proporcionado no coincide con el Id de la marca."
                    });
                }

                brand.Name = dto.Name;

                await _brandService.UpdateBrandAsync(brand);

                return Ok(new
                {
                    message = "Marca actualizada correctamente."
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

        // DELETE: api/brands/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrandAsync(int id)
        {
            try
            {
                await _brandService.DesactivateBrandAsync(id);

                return Ok(new
                {
                    message = "Marca eliminada correctamente."
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
        // ACTIVTE: api/brands/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> ActivateBrandAsync(int id)
        {
            try
            {
                await _brandService.ActivateBrandAsync(id);

                return Ok(new
                {
                    message = "Marca activada correctamente."
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
