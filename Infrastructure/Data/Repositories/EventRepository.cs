using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ISession _session;

        public EventRepository(ISession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }
        public IEnumerable<MotorcycleRegisteredEvent> GetAll()
        {
            return _session.Query<MotorcycleRegisteredEvent>().ToList();
        }

        public async Task<IEnumerable<MotorcycleRegisteredEvent>> GetAllAsync()
        {
            return await _session.Query<MotorcycleRegisteredEvent>().ToListAsync();
        }

        public MotorcycleRegisteredEvent GetById(int id)
        {
            return _session.Get<MotorcycleRegisteredEvent>(id);
        }

        public async Task<MotorcycleRegisteredEvent> GetByIdAsync(int id)
        {
            return await _session.GetAsync<MotorcycleRegisteredEvent>(id);
        }

        public void Add(MotorcycleRegisteredEvent entity)
        {
            _session.Save(entity);
            _session.Flush();
        }

        public async Task AddAsync(MotorcycleRegisteredEvent entity)
        {
            await _session.SaveAsync(entity);
            await _session.FlushAsync();
        }

        public void Update(MotorcycleRegisteredEvent entity)
        {
            _session.Update(entity);
            _session.Flush();
        }

        public async Task UpdateAsync(MotorcycleRegisteredEvent entity)
        {
            await _session.UpdateAsync(entity);
            await _session.FlushAsync();
        }

        public void Delete(MotorcycleRegisteredEvent entity)
        {
            _session.Delete(entity);
            _session.Flush();
        }

        public async Task DeleteAsync(MotorcycleRegisteredEvent entity)
        {
            await _session.DeleteAsync(entity);
            await _session.FlushAsync();
        }

        public async Task<IEnumerable<MotorcycleRegisteredEvent>> GetEventsByMotorcycleAsync(int motorcycleId)
        {
            return await _session.Query<MotorcycleRegisteredEvent>()
                .Where(e => e.Motorcycle.Id == motorcycleId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MotorcycleRegisteredEvent>> Get2024ModelEventsAsync()
        {
            return await _session.Query<MotorcycleRegisteredEvent>()
                .Where(e => e.Is2024Model)
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();
        }
    }
}