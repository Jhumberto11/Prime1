using API_PRIMECRM.Domain.Interfaces;
using API_PRIMECRM.Domain.Models.Masters;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Application.Master
{
    public class ProductService
    {
        private readonly IBaseRepository<Product> _productRepository;

        public ProductService(IBaseRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El Id del producto debe ser mayor que cero.");
            }
            
            var product = await _productRepository.GetByIdAsync(id);
            if(product == null)
            {
                throw new KeyNotFoundException($"No existe un producto con Id {id}.");
            }

            return product;
        }


        public async Task<Product> AddProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (await _productRepository.ExistsAsync(
                    x => x.Name.ToLower() == product.Name.ToLower()))
            {
                throw new InvalidOperationException
                (
                    $"El producto '{product.Name}' ya existe."
                );
            }
            ValidateProduct(product);
            product.Name = product.Name.Trim();
            await _productRepository.AddAsync(product);
            return product;
        }


        public async Task<Product> UpdateProductAsync(Product UpdateProduct)
        {
            if (UpdateProduct == null)
                throw new ArgumentNullException(nameof(UpdateProduct));
            if (UpdateProduct.Id <= 0)
                throw new ArgumentException("El Id del producto debe ser mayor que cero.");
            var existingProduct = await _productRepository.GetByIdAsync(UpdateProduct.Id);
            if (existingProduct == null)
                throw new KeyNotFoundException($"No existe un producto con Id {UpdateProduct.Id}.");
            if (await _productRepository.ExistsAsync(
                    x => x.Name.ToLower() == UpdateProduct.Name.ToLower() && x.Id != UpdateProduct.Id))
            {
                throw new InvalidOperationException
                (
                    $"El producto con nombre '{UpdateProduct.Name}' ya existe, utiliza otro nombre."
                );
            }
            
            existingProduct.Name = UpdateProduct.Name.Trim();
            await _productRepository.Update(existingProduct);
            return existingProduct;
        }


        public async Task DeleteProductAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El Id del producto debe ser mayor que cero.");

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"No existe un producto con Id {id}.");

            await _productRepository.Delete(product);
        }

        private static void ValidateProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException(
                    "El nombre del producto es obligatorio."
                );
            
            if (product.Name.Trim().Length > 100)
                throw new ArgumentException(
                    "El nombre del producto no puede superar los 100 caracteres."
                );
        }
    }


}
