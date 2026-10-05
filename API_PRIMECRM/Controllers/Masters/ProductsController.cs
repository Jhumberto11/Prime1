using API_PRIMECRM.Application.Services.Master;
using API_PRIMECRM.Domain.DTOs.Masters;
using API_PRIMECRM.Domain.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_PRIMECRM.Controllers.Masters
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();

            if(products == null || !products.Any())
            {
                return NotFound(new { message = "No hay productos registrados" });
            }
            return Ok(products);
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                return Ok(product);
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
        public async Task<IActionResult> CreateProduct(ProductDto product)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(new
                {
                    message = "El modelo de datos no es válido."
                });
            }
            if(product.BrandId <= 0)
            {
                return BadRequest(new
                {
                    message = "El Id de la marca debe ser mayor a cero."
                });
            }

            Product newProduct  = new Product
            {
                Name = product.Name,
                BrandId = product.BrandId
            };

            try
            {
                var createdProduct = await _productService.AddProductAsync(newProduct);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdProduct.Id },
                    createdProduct
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

        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(
            int id,
            ProductDto product)
        {
            var productToUpdate = await _productService.GetProductByIdAsync(id);
            if(productToUpdate == null)
            {
                return NotFound(new
                {
                    message = "El producto no existe."
                });
            }

            try
            {
                if (id != productToUpdate.Id)
                {
                    return BadRequest(new
                    {
                        message = "El Id de la URL no coincide con el Id del producto."
                    });
                }

                productToUpdate.Name = product.Name;
                productToUpdate.BrandId = product.BrandId;

                await _productService.UpdateProductAsync(productToUpdate);


                return Ok(new
                {
                    message = "Producto actualizado correctamente."
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
        [HttpDelete("desactivate/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DesactivateProductAsync(id);

                return Ok(new
                {
                    message = "Producto eliminado correctamente."
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
        public async Task<IActionResult> ActivateProduct(int id)
        {
            try
            {
                await _productService.ActivateProductAsync(id);

                return Ok(new
                {
                    message = "Producto activado correctamente."
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
