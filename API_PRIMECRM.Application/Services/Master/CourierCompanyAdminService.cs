using API_PRIMECRM.Domain.Interfaces;
using API_PRIMECRM.Domain.Models.Masters;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Application.Services.Master
{
    public class CourierCompanyAdminService
    {
        private readonly IBaseRepository<CourierCompany> _courierCompanyRepository;
        public CourierCompanyAdminService(IBaseRepository<CourierCompany> courierCompanyRepository)
        {
            _courierCompanyRepository = courierCompanyRepository;
        }


        public async Task<IEnumerable<CourierCompany>> GetAllCourierCompanyAsync()
        {
            return await _courierCompanyRepository.GetAllAsync();
        }

        public async Task<IEnumerable<CourierCompany>> GetActiveCourierCompanyAsync()
        {
            return await _courierCompanyRepository.GetAllActivatedAsync();
        }


        public async Task<CourierCompany?> GetCourierCompanyByIdAsync(int id)
        {

            if (id <= 0)
            {
                throw new ArgumentException("El Id de la compañía de transporte debe ser mayor que cero.");
            }

            var courierCompany = await _courierCompanyRepository.GetByIdAsync(id);

            if (courierCompany == null)
                throw new KeyNotFoundException($"No existe una compañía de transporte con Id {id}.");

            return courierCompany;
        }

        public async Task<CourierCompany> AddCourierCompanyAsync(CourierCompany courierCompany)
        {

            if (courierCompany == null)
                throw new ArgumentNullException(nameof(courierCompany));
            ///
            ///
  
            if (courierCompany.CashHandlingType <= 0)
                throw new ArgumentException("El tipo de manejo de efectivo no debe estar vacío.");
            if (courierCompany.CashHandlingValue <= 0)
                throw new ArgumentException("El porcentaje del manejo de efectivo debe ser mayor que cero.");
            

            if (await _courierCompanyRepository.ExistsAsync(
                    x => x.Name.ToLower() == courierCompany.Name.ToLower()))
            {
                throw new InvalidOperationException
                (
                    $"La compañía de transporte '{courierCompany.Name}' ya existe."
                );
            }

            ValidateCourierCompany(courierCompany);

            courierCompany.Name = courierCompany.Name.Trim();

            await _courierCompanyRepository.AddAsync(courierCompany);

            return courierCompany;
        }

        public async Task UpdateCourierCompanyAsync(CourierCompany courierCompany)
        {
            if (courierCompany == null)
                throw new ArgumentNullException(nameof(courierCompany   ));

            if (courierCompany.Id <= 0)
                throw new ArgumentException("El Id de la compañía de transporte debe ser mayor que cero.");

            ValidateCourierCompany(courierCompany);

            var existingCourierCompany = await _courierCompanyRepository.GetByIdAsync(courierCompany.Id);

            if (existingCourierCompany == null)
                throw new KeyNotFoundException(
                    $"No existe una compañía de transporte con Id {courierCompany.Id}."
                );

            existingCourierCompany.Name = courierCompany.Name.Trim();
            existingCourierCompany.CashHandlingValue = courierCompany.CashHandlingValue;
            existingCourierCompany.DeliveryRate = courierCompany.DeliveryRate;
            existingCourierCompany.CashHandlingType = courierCompany.CashHandlingType;
            existingCourierCompany.IsActive = courierCompany.IsActive;

            await _courierCompanyRepository.Update(existingCourierCompany);
        }

        public async Task DesactivateCourierCompanyAsync(int courierCompanyId)
        {
            if (courierCompanyId <= 0)
                throw new ArgumentException("El Id de la compañía de transporte debe ser mayor que cero.");

            var courierCompany = await _courierCompanyRepository.GetByIdAsync(courierCompanyId);

            if (courierCompany == null)
                throw new KeyNotFoundException(
                    $"No existe una compañía de transporte con Id : {courierCompanyId}."
                );

            courierCompany.IsActive = false;

            await _courierCompanyRepository.Update(courierCompany);

            
        }
        public async Task ActivateCourierCompanyAsync(int courierCompanyId)
        {
            if (courierCompanyId <= 0)
                throw new ArgumentException("El Id de la compañía de transporte debe ser mayor que cero.");

            var courierCompany = await _courierCompanyRepository.GetByIdAsync(courierCompanyId);

            if (courierCompany == null)
                throw new KeyNotFoundException(
                    $"No existe una compañía de transporte con Id : {courierCompanyId}."
                );

            courierCompany.IsActive = true;
            await _courierCompanyRepository.Update(courierCompany);
        }

        private static void ValidateCourierCompany(CourierCompany courierCompany)
        {
            if (string.IsNullOrWhiteSpace(courierCompany.Name))
                throw new ArgumentException(
                    "El nombre de la compañía de transporte es obligatorio."
                );
            if (string.IsNullOrWhiteSpace(courierCompany.CashHandlingValue.ToString()))
                throw new ArgumentException(
                    "La % comision por envio es obligatoria. (CashHandlingValue)"
                );
            if (string.IsNullOrWhiteSpace(courierCompany.DeliveryRate.ToString()))
                throw new ArgumentException(
                    "La tarifa de entrega es obligatoria. (DeliveryRate)"
                );
            if (string.IsNullOrWhiteSpace(courierCompany.CashHandlingType.ToString()))
                throw new ArgumentException(
                    "El tipo de manejo de efectivo es obligatorio. (CashHandlingType)"
                );

            if (courierCompany.Name.Trim().Length > 100)
                throw new ArgumentException(
                    "El nombre de la compañía de transporte no puede superar los 100 caracteres."
                );
        }


    }
}
