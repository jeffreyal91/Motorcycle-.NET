using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IMotorcycleService
    {
        Task<Motorcycle> GetByIdAsync(int id);
        Task<IEnumerable<Motorcycle>> GetAllAsync();
        Task<bool> ExistsAsync(int id);
        Task<bool> HasMotorcyclesFromYearAsync(int year);
        Task<Motorcycle> CreateMotorcycleAsync(string id, int year, string model, string licensePlate);
        Task AddMotorcycle(Motorcycle motorcycle);
        Task<IEnumerable<Motorcycle>> GetByLicensePlateAsync(string licensePlate);
        Task<Motorcycle?> UpdateLicensePlateAsync(int id, string newLicensePlate);
        Task<bool> DeleteMotorcycleAsync(int id);
    }
}