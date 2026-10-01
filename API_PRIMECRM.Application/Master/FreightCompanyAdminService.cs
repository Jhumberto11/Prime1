using API_PRIMECRM.Domain.Interfaces;
using API_PRIMECRM.Domain.Models.Masters;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Application.Master
{
    public class FreightCompanyAdminService
    {
        private readonly IBaseRepository<FreightCompany> _repository;
        public FreightCompanyAdminService(IBaseRepository<FreightCompany> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<FreightCompany>> GetAllFreightCompanyAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<FreightCompany?> GetFreightCompanyByIdAsync(int id)
        {

            if (id <= 0)
            {
                throw new ArgumentException("El Id de la compañía de transporte debe ser mayor que cero.");
            }

            var freightCompany = await _repository.GetByIdAsync(id);

            if (freightCompany == null)
                throw new KeyNotFoundException($"No existe una compañía de transporte con Id {id}.");

            return freightCompany;
        }

        public async Task<FreightCompany> AddFreightCompanyAsync(FreightCompany freightCompany)
        {

            if (freightCompany == null)
                throw new ArgumentNullException(nameof(freightCompany));

            if (freightCompany.RatePerLB <= 0)
                throw new ArgumentException("La tarifa por libra debe ser mayor que cero.");
            if (freightCompany.TaxPercentSV <= 0)
                throw new ArgumentException("El porcentaje de impuesto debe ser mayor que cero.");
            if (freightCompany.OtherCharges <= 0)
                throw new ArgumentException("Las cargas adicionales deben ser un valor positivo .");

            if (await _repository.ExistsAsync(
                    x => x.Name.ToLower() == freightCompany.Name.ToLower()))
            {
                throw new InvalidOperationException
                (
                    $"La compañía de transporte '{freightCompany.Name}' ya existe."
                );
            }

            ValidateFreightCompany(freightCompany);

            freightCompany.Name = freightCompany.Name.Trim();

            await _repository.AddAsync(freightCompany);

            return freightCompany;
        }

        public async Task UpdateFreightCompanyAsync(FreightCompany freightCompany)
        {
            if (freightCompany == null)
                throw new ArgumentNullException(nameof(freightCompany));

            if (freightCompany.Id <= 0)
                throw new ArgumentException("El Id de la compañía de transporte debe ser mayor que cero.");

            ValidateFreightCompany(freightCompany);

            var existingFreightCompany = await _repository.GetByIdAsync(freightCompany.Id);

            if (existingFreightCompany == null)
                throw new KeyNotFoundException(
                    $"No existe una compañía de transporte con Id {freightCompany.Id}."
                );

            existingFreightCompany.Name = freightCompany.Name.Trim();
            existingFreightCompany.RatePerLB = freightCompany.RatePerLB;
            existingFreightCompany.TaxPercentSV = freightCompany.TaxPercentSV;
            existingFreightCompany.OtherCharges = freightCompany.OtherCharges;
            existingFreightCompany.IsActive = freightCompany.IsActive;

            await _repository.Update(existingFreightCompany);
        }

        public async Task DesactivateFreightCompanyAsync(int freightCompanyId)
        {
            if (freightCompanyId <= 0)
                throw new ArgumentException("El Id de la compañía de transporte debe ser mayor que cero.");

            var freightCompany = await _repository.GetByIdAsync(freightCompanyId);

            if (freightCompany == null)
                throw new KeyNotFoundException(
                    $"No existe una compañía de transporte con Id : {freightCompanyId}."
                );

            freightCompany.IsActive = false;

            await _repository.Update(freightCompany);


        }
        public async Task ActivateFreightCompanyAsync(int freightCompanyId)
        {
            if (freightCompanyId <= 0)
                throw new ArgumentException("El Id de la compañía de transporte debe ser mayor que cero.");

            var freightCompany = await _repository.GetByIdAsync(freightCompanyId);

            if (freightCompany == null)
                throw new KeyNotFoundException(
                    $"No existe una compañía de transporte con Id : {freightCompanyId}."
                );

            freightCompany.IsActive = true;
            await _repository.Update(freightCompany);
        }

        private static void ValidateFreightCompany(FreightCompany freightCompany)
        {
            if (string.IsNullOrWhiteSpace(freightCompany.Name))
                throw new ArgumentException(
                    "El nombre de la compañía de transporte es obligatorio."
                );
            if (string.IsNullOrWhiteSpace(freightCompany.RatePerLB.ToString())) 
                throw new ArgumentException(
                    "La tarifa por libra es obligatoria. (RatePerLB)"
                );
            if (string.IsNullOrWhiteSpace(freightCompany.TaxPercentSV.ToString())) 
                throw new ArgumentException(
                    "El porcentaje de impuesto es obligatorio. (TaxPercentSV)"
                );
            if (string.IsNullOrWhiteSpace(freightCompany.OtherCharges.ToString())) 
                throw new ArgumentException(
                    "Las cargas adicionales o costos adiccionales son obligatorios. (OtherCharges)"
                );

            if (freightCompany.Name.Trim().Length > 100)
                throw new ArgumentException(
                    "El nombre de la compañía de transporte no puede superar los 100 caracteres."
                );
        }

    }
}
