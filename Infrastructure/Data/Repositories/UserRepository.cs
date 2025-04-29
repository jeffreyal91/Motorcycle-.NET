using Domain.Entities;
using Domain.Interfaces;
using NHibernate;

namespace Infrastructure.Data.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly ISession _session;

        public UserRepository(ISession session)
        {
            _session = session;
        }

        public User GetById(int id)
        {
            return _session.Get<User>(id);
        }

        public IEnumerable<User> GetAll()
        {
            return _session.Query<User>().ToList();
        }

        public void Add(User entity)
        {
            using var transaction = _session.BeginTransaction();
            _session.Save(entity);
            transaction.Commit();
        }

        public void Update(User entity)
        {
            using var transaction = _session.BeginTransaction();
            _session.Update(entity);
            transaction.Commit();
        }

        public void Delete(User entity)
        {
            using var transaction = _session.BeginTransaction();
            _session.Delete(entity);
            transaction.Commit();
        }

        public Task<User> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
