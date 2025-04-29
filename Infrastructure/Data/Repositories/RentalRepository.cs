using Domain.Entities;
using Domain.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class RentalRepository : IRentalRepository
    {
        private readonly ISession _session;

        public RentalRepository(ISession session)
        {
            _session = session;
        }

        public async Task<Rental> GetByIdAsync(int id)
        {
            return await _session.GetAsync<Rental>(id);
        }

        public async Task<IEnumerable<Rental>> GetAllAsync()
        {
            return await _session.Query<Rental>()
                .Fetch(r => r.DeliveryDriver)
                .Fetch(r => r.Motorcycle)
                .ToListAsync();
        }

        public async Task<IEnumerable<Rental>> GetByMotorcycleIdAsync(int motorcycleId)
        {
            return await _session.Query<Rental>()
                .Where(r => r.Motorcycle.Id == motorcycleId)
                .Fetch(r => r.DeliveryDriver)
                .ToListAsync();
        }

        public async Task<IEnumerable<Rental>> GetByDriverIdAsync(int driverId)
        {
            return await _session.Query<Rental>()
                .Where(r => r.DeliveryDriver.Id == driverId)
                .Fetch(r => r.Motorcycle)
                .ToListAsync();
        }

        public async Task<bool> HasRentalsForMotorcycleAsync(int motorcycleId)
        {
            return await _session.Query<Rental>()
                .AnyAsync(r => r.Motorcycle.Id == motorcycleId && r.ActualEndDate == null);
        }

        public async Task<bool> HasActiveRentalForDriverAsync(int driverId)
        {
            return await _session.Query<Rental>()
                .AnyAsync(r => r.DeliveryDriver.Id == driverId && r.ActualEndDate == null);
        }

        public async Task AddAsync(Rental rental)
        {
            await _session.SaveAsync(rental);
            await _session.FlushAsync();
        }

        public async Task UpdateAsync(Rental rental)
        {
            await _session.UpdateAsync(rental);
            await _session.FlushAsync();
        }

        public async Task DeleteAsync(Rental rental)
        {
            await _session.DeleteAsync(rental);
            await _session.FlushAsync();
        }
    }
}