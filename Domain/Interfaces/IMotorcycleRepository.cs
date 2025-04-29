using Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IMotorcycleRepository
    {
        IQueryable<Motorcycle> GetAll();
        Task<Motorcycle> GetByIdAsync(int id);
        Task AddAsync(Motorcycle motorcycle);
        Task UpdateAsync(Motorcycle motorcycle);
        Task DeleteAsync(Motorcycle motorcycle);
    }
}
