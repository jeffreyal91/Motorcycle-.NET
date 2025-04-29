using Domain.Entities;
using Domain.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class MotorcycleRepository : IMotorcycleRepository
    {
        protected readonly ISession _session;

        public MotorcycleRepository(ISession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public IEnumerable<Motorcycle> GetAll()
        {
            return _session.Query<Motorcycle>().ToList();
        }

        public Motorcycle GetById(int id)
        {
            return _session.Get<Motorcycle>(id);
        }

        public void Add(Motorcycle entity)
        {
            _session.Save(entity);
            _session.Flush();
        }

        public void Update(Motorcycle entity)
        {
            _session.Update(entity);
            _session.Flush();
        }

        public void Delete(Motorcycle entity)
        {
            _session.Delete(entity);
            _session.Flush();
        }

        public async Task<IEnumerable<Motorcycle>> GetAllAsync()
        {
            return await _session.Query<Motorcycle>().ToListAsync();
        }

        public async Task<Motorcycle> GetByIdAsync(int id)
        {
            return await _session.GetAsync<Motorcycle>(id);
        }

        public async Task AddAsync(Motorcycle entity)
        {
            await _session.SaveAsync(entity);
            await _session.FlushAsync();
        }

        public async Task UpdateAsync(Motorcycle entity)
        {
            await _session.UpdateAsync(entity);
            await _session.FlushAsync();
        }

        public async Task DeleteAsync(Motorcycle entity)
        {
            await _session.DeleteAsync(entity);
            await _session.FlushAsync();
        }

        public async Task<bool> ExistsByLicensePlateAsync(string licensePlate)
        {
            return await _session.Query<Motorcycle>()
                .AnyAsync(m => m.LicensePlate == licensePlate.ToUpper());
        }

        public async Task<IEnumerable<Motorcycle>> GetByYearAsync(int year)
        {
            return await _session.Query<Motorcycle>()
                .Where(m => m.Year == year)
                .ToListAsync();
        }

        public async Task<Motorcycle> GetByLicensePlateAsync(string licensePlate)
        {
            return await _session.Query<Motorcycle>()
                .FirstOrDefaultAsync(m => m.LicensePlate == licensePlate.ToUpper());
        }
    }
}