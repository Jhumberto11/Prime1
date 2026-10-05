using API_PRIMECRM.Domain.Interfaces;
using API_PRIMECRM.Domain.Models.Masters;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Application.Services.Master
{
    public class BrandService
    {
        private readonly IBaseRepository<Brand> _brandRepository;
        public BrandService(IBaseRepository<Brand> brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
        {
            

            return await _brandRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Brand>> GetActiveBrandsAsync()
        {
            return await _brandRepository.GetAllActivatedAsync();
        }

        public async Task<Brand?> GetBrandByIdAsync(int id)
        {

            if (id <= 0)
            {
                throw new ArgumentException("El Id de la marca debe ser mayor que cero.");
            }

            var brand = await _brandRepository.GetByIdAsync(id);

            if (brand == null)
                throw new KeyNotFoundException($"No existe una marca con Id {id}.");

            return brand;
        }

        public async Task<Brand> AddBrandAsync(Brand brand)
        {

            if (brand == null)
                throw new ArgumentNullException(nameof(brand));

            if (await _brandRepository.ExistsAsync(
                    x => x.Name.ToLower() == brand.Name.ToLower()))
            {
                throw new InvalidOperationException
                (
                    $"La marca '{brand.Name}' ya existe."
                );
            }

            ValidateBrand(brand);

            brand.Name = brand.Name.Trim();

            await _brandRepository.AddAsync(brand);

            return brand;
        }

        public async Task UpdateBrandAsync(Brand brand)
        {
            if (brand == null)
                throw new ArgumentNullException(nameof(brand));

            if (brand.Id <= 0)
                throw new ArgumentException("El Id de la marca debe ser mayor que cero.");

            ValidateBrand(brand);

            var existingBrand = await _brandRepository.GetByIdAsync(brand.Id);

            if (existingBrand == null)
                throw new KeyNotFoundException(
                    $"No existe una marca con Id {brand.Id}."
                );

            existingBrand.Name = brand.Name.Trim();
            existingBrand.IsActive = brand.IsActive;

            await _brandRepository.Update(existingBrand);
        }

        public async Task DesactivateBrandAsync(int brandId)
        {
            if (brandId <= 0)
                throw new ArgumentException("El Id de la marca debe ser mayor que cero.");

            var brand = await _brandRepository.GetByIdAsync(brandId);

            if (brand == null)
                throw new KeyNotFoundException(
                    $"No existe una marca con Id {brandId}."
                );

            brand.IsActive = false;
            await _brandRepository.Update(brand);
        }
        public async Task ActivateBrandAsync(int brandId)
        {
            if (brandId <= 0)
                throw new ArgumentException("El Id de la marca debe ser mayor que cero.");

            var brand = await _brandRepository.GetByIdAsync(brandId);

            if (brand == null)
                throw new KeyNotFoundException(
                    $"No existe una marca con Id {brandId}."
                );

            brand.IsActive = true;
            await _brandRepository.Update(brand);
        }

        private static void ValidateBrand(Brand brand)
        {
            if (string.IsNullOrWhiteSpace(brand.Name))
                throw new ArgumentException(
                    "El nombre de la marca es obligatorio."
                );

            if (brand.Name.Trim().Length > 100)
                throw new ArgumentException(
                    "El nombre de la marca no puede superar los 100 caracteres."
                );
        }
    }
}
