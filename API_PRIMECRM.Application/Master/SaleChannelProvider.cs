using API_PRIMECRM.Domain.Interfaces;
using API_PRIMECRM.Domain.Models.Masters;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Application.Master
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
                throw new Exception($"Sales channel with ID {id} not found.");
            }
            return channel;
        }
    }
}
