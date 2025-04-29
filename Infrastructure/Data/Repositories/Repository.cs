using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using NHibernate;
using NHibernate.Linq;


namespace Infrastructure.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : EntityBase
    {
        protected readonly ISession _session;

        public Repository(ISession session)
        {
            _session = session;
        }

        // Async implementations
        public async Task<T> GetByIdAsync(int id)
        {
            return await _session.GetAsync<T>(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _session.Query<T>().ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _session.SaveAsync(entity);
            await _session.FlushAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            await _session.UpdateAsync(entity);
            await _session.FlushAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            await _session.DeleteAsync(entity);
            await _session.FlushAsync();
        }

        // Sync implementations
        public T GetById(int id)
        {
            return _session.Get<T>(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _session.Query<T>().ToList();
        }

        public void Add(T entity)
        {
            _session.Save(entity);
            _session.Flush();
        }

        public void Update(T entity)
        {
            _session.Update(entity);
            _session.Flush();
        }

        public void Delete(T entity)
        {
            _session.Delete(entity);
            _session.Flush();
        }
    }
}