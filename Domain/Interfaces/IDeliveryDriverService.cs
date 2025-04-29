using Domain.Entities;
using Domain.Enums;
using System;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IDeliveryDriverService
    {
        Task<DeliveryDriver> RegisterDriverAsync(
            string identify,
            string fullName,
            string cnpj,
            DateTime birthDate,
            string driverLicenseNumber,
            DriverLicenseType driverLicenseType,
            string driverLicenseImagePath);

        Task<DeliveryDriver?> GetDriverByIdAsync(Guid id);
    }
}