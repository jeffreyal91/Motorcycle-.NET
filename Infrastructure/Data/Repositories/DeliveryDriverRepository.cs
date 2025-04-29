using Domain.Entities;
using Domain.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class DeliveryDriverRepository : IDeliveryDriverRepository
    {
        private readonly ISession _session;

        public DeliveryDriverRepository(ISession session)
        {
            _session = session;
        }

        public async Task AddAsync(DeliveryDriver driver)
        {
            using var transaction = _session.BeginTransaction();
            try
            {
                await _session.SaveAsync(driver);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<DeliveryDriver?> GetByIdAsync(Guid id)
        {
            return await _session.GetAsync<DeliveryDriver>(id);
        }

        public async Task<DeliveryDriver?> GetByCnpjAsync(string cnpj)
        {
            return await _session.Query<DeliveryDriver>()
                .FirstOrDefaultAsync(d => d.CNPJ == cnpj);
        }

        public async Task<DeliveryDriver?> GetByDriverLicenseAsync(string licenseNumber)
        {
            return await _session.Query<DeliveryDriver>()
                .FirstOrDefaultAsync(d => d.DriverLicenseNumber == licenseNumber);
        }
    }
}