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
        public SaleChannelProvider(IBaseRepository<SalesChannel> saleChannelRepository)
        {
            _saleChannelRepository = saleChannelRepository;
        }

        public async Task<IEnumerable<SalesChannel>> GetAllSaleChannelsAsync()
        {
            return await _saleChannelRepository.GetAllAsync();
        }
        public async Task<SalesChannel> GetSaleChannelAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El Id del canal de venta debe ser mayor que cero.");
            }
            var channel = await _saleChannelRepository.GetByIdAsync(id);

            if (channel == null)
            {
                throw new KeyNotFoundException($"Sales channel with ID {id} not found.");
            }
            return channel;
        }

        public async Task<SalesChannel> AddSaleChannelAsync(SalesChannel channel)
        {
            if (channel == null)
            {
                throw new ArgumentNullException(nameof(channel));
            }

            if (string.IsNullOrWhiteSpace(channel.Name))
            {
                throw new ArgumentException(
                    "El nombre del canal de venta es requerido.",
                    nameof(channel));
            }

            channel.Name = channel.Name.Trim();

            if (await _saleChannelRepository.ExistsAsync(
                x => x.Name.ToLower() == channel.Name.ToLower()))
            {
                throw new InvalidOperationException(
                    $"El canal de venta '{channel.Name}' ya existe.");
            }

            await _saleChannelRepository.AddAsync(channel);

            return channel;
        }

        public async Task<SalesChannel> UpdateSaleChannelAsync(SalesChannel channel)
        {
            if (channel == null)
            {
                throw new ArgumentNullException(nameof(channel));
            }

            if (channel.Id <= 0)
            {
                throw new ArgumentException(
                    "El Id del canal de venta debe ser mayor que cero.",
                    nameof(channel));
            }

            if (string.IsNullOrWhiteSpace(channel.Name))
            {
                throw new ArgumentException(
                    "El nombre del canal de venta es requerido.",
                    nameof(channel));
            }

            var existingChannel =
                await _saleChannelRepository.GetByIdAsync(channel.Id);

            if (existingChannel == null)
            {
                throw new KeyNotFoundException(
                    $"No existe un canal de venta con Id {channel.Id}.");
            }

            var normalizedName = channel.Name.Trim();

            var duplicateExists =
                await _saleChannelRepository.ExistsAsync(
                    x => x.Id != channel.Id &&
                         x.Name.ToLower() == normalizedName.ToLower());

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    $"Ya existe un canal de venta llamado '{normalizedName}'.");
            }

            existingChannel.Name = normalizedName;
            await _saleChannelRepository.Update(existingChannel);
            return existingChannel;
        }

        public async Task DeactivateSaleChannelAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El Id del canal de venta debe ser mayor que cero.");
            }
            var channel = await _saleChannelRepository.GetByIdAsync(id);
            if (channel == null)
            {
                throw new KeyNotFoundException($"No existe un canal de venta con Id {id}.");
            }
            if (!channel.IsActive)
            {
                return;
            }
            channel.IsActive = false;
            await _saleChannelRepository.Update(channel);
        }
        public async Task ActivateSaleChannelAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El Id del canal de venta debe ser mayor que cero.");
            }
            var channel = await _saleChannelRepository.GetByIdAsync(id);
            if (channel == null)
            {
                throw new KeyNotFoundException($"No existe un canal de venta con Id {id}.");
            }
            if (channel.IsActive)
            {
                return;
            }
            channel.IsActive = true;
            await _saleChannelRepository.Update(channel);
        }
    }
}
