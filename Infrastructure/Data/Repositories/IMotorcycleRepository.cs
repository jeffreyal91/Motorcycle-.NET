using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;


public interface IMotorcycleRepository : IRepository<Motorcycle>
{
    Task<bool> ExistsByLicensePlateAsync(string licensePlate);
    Task<IEnumerable<Motorcycle>> GetByYearAsync(int year);
    Task<Motorcycle> GetByLicensePlateAsync(string licensePlate);
    Task<IEnumerable<Motorcycle>> GetAllAsync();
    Task<Motorcycle?> GetByIdAsync(int id);
    Task UpdateAsync(Motorcycle motorcycle);
    Task DeleteAsync(Motorcycle motorcycle);
}
