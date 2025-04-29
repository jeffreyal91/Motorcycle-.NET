using Domain.Entities;
using FluentNHibernate.Mapping;

namespace Infrastructure.Data.Mappings
{
    public class MotorcycleMap : ClassMap<Motorcycle>
    {
        public MotorcycleMap()
        {
            Table("motorcycles");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Identifier).Not.Nullable();
            Map(x => x.Year).Not.Nullable().Column("year");
            Map(x => x.Model).Not.Nullable().Length(100);
            Map(x => x.LicensePlate).Not.Nullable().Length(15);
            Map(x => x.CreatedAt).Not.Nullable();
        }
    }
}