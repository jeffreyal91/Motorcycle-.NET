using Domain.Entities;
using Domain.Events;


 public interface IRepository<T> where T : EntityBase
    {
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        T GetById(int id);
        Task<T> GetByIdAsync(int id);
        void Add(T entity);
        Task AddAsync(T entity);
        void Update(T entity);
        Task UpdateAsync(T entity);
        void Delete(T entity);
        Task DeleteAsync(T entity);
    }
    

public interface IEventRepository : IRepository<MotorcycleRegisteredEvent>
{
    Task<IEnumerable<MotorcycleRegisteredEvent>> GetEventsByMotorcycleAsync(int motorcycleId);
    Task<IEnumerable<MotorcycleRegisteredEvent>> Get2024ModelEventsAsync();
}