using System;
using Domain.Enums;

namespace Domain.Entities
{
    public class DeliveryDriver : EntityBase
    {
        public virtual string Identifier { get; set; }
        public virtual required string FullName { get; set; }
        public virtual required string CNPJ { get; set; }
        public virtual DateTime BirthDate { get; set; }
        public virtual required string DriverLicenseNumber { get; set; }
        public virtual DriverLicenseType DriverLicenseType { get; set; }
        public virtual required string DriverLicenseImagePath { get; set; }
        public virtual DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

        //  1:N with Rentals
        public virtual IList<Rental> Rentals { get; protected set; } = new List<Rental>();


        public virtual bool CanRentMotorcycles()
        {
            return DriverLicenseType == DriverLicenseType.A ||
                   DriverLicenseType == DriverLicenseType.AB;
        }
    }
}