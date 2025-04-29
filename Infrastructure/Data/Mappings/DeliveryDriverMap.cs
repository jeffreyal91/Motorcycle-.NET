using Domain.Entities;
using Domain.Enums;
using FluentNHibernate.Mapping;

namespace Infrastructure.Data.Mappings
{
    public class DeliveryDriverMap : ClassMap<DeliveryDriver>
    {
        public DeliveryDriverMap()
        {
            Table("delivery_drivers");
            Id(x => x.Id).GeneratedBy.Identity();

            Map(x => x.Identifier);
            Map(x => x.FullName).Not.Nullable().Length(100);
            Map(x => x.CNPJ).Not.Nullable().Length(14).Unique();
            Map(x => x.BirthDate).Not.Nullable();
            Map(x => x.DriverLicenseNumber).Not.Nullable().Length(20).Unique();
            Map(x => x.DriverLicenseType).Not.Nullable().CustomType<DriverLicenseType>().Column("driver_license_type").CustomSqlType("varchar(2)");
            Map(x => x.DriverLicenseImagePath).Not.Nullable().Length(500);
            Map(x => x.CreatedAt).Not.Nullable();

            HasMany(x => x.Rentals)
                .KeyColumn("delivery_driver_id")
                .Inverse()
                .Cascade.AllDeleteOrphan();
        }
    }
}