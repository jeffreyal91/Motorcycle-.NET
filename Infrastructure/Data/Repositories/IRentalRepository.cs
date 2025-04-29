using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRentalRepository
    {
       Task<Rental> GetByIdAsync(int id);
        Task<IEnumerable<Rental>> GetAllAsync();
        Task<IEnumerable<Rental>> GetByMotorcycleIdAsync(int motorcycleId);
        Task<IEnumerable<Rental>> GetByDriverIdAsync(int driverId);
        Task<bool> HasRentalsForMotorcycleAsync(int motorcycleId);
        Task<bool> HasActiveRentalForDriverAsync(int driverId);
        Task AddAsync(Rental rental);
        Task UpdateAsync(Rental rental);
        Task DeleteAsync(Rental rental);
    }
}