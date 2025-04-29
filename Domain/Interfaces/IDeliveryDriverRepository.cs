using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IDeliveryDriverRepository
    {
        Task<DeliveryDriver?> GetByIdAsync(Guid id);
        Task<DeliveryDriver?> GetByCnpjAsync(string cnpj);
        Task<DeliveryDriver?> GetByDriverLicenseAsync(string licenseNumber);
        Task AddAsync(DeliveryDriver driver);
    }
}