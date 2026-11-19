using API_PRIMECRM.Domain.Interfaces;
using API_PRIMECRM.Domain.Models.Masters;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Application.Services.Master
{
    public class SaleChannelProvider
    {
        private readonly IBaseRepository<SalesChannel> _saleChannelRepository;

        public SaleChannelProvider(
            IBaseRepository<SalesChannel> saleChannelRepository)
        {
            _saleChannelRepository = saleChannelRepository;
        }

        public async Task<IEnumerable<SalesChannel>>
            GetAllSaleChannelsAsync()
        {
            return await _saleChannelRepository.GetAllAsync();
        }

        public async Task<IEnumerable<SalesChannel>>
            GetActiveSaleChannelsAsync()
        {
            return await _saleChannelRepository.GetAllActivatedAsync();
        }

        public async Task<SalesChannel>
            GetSaleChannelAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "El Id del canal de venta debe ser mayor que cero.");
            }

            var channel =
                await _saleChannelRepository.GetByIdAsync(id);

            if (channel == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró el canal de venta con Id {id}.");
            }

            return channel;
        
        }

        public async Task<SalesChannel>AddSaleChannelAsync(SalesChannel channel)
        {
        if (channel == null)
            throw new ArgumentNullException(nameof(channel));

        ValidateSaleChannel(channel);

        if (await _saleChannelRepository.ExistsAsync(
                x => x.Name.ToLower() ==
                     channel.Name.ToLower()))
        {
            throw new InvalidOperationException(
                $"El canal de venta '{channel.Name}' ya existe.");
        }

        channel.Name = channel.Name.Trim();

        await _saleChannelRepository.AddAsync(channel);

        return channel;
    }

    

    private static void ValidateSaleChannel(
        SalesChannel channel)
    {
        if (string.IsNullOrWhiteSpace(channel.Name))
        {
            throw new ArgumentException(
                "El nombre del canal de venta es obligatorio.");
        }

        if (channel.Name.Trim().Length > 100)
        {
            throw new ArgumentException(
                "El nombre del canal de venta no puede superar los 100 caracteres.");
        }
    }
}
}
