using Domain.Entities;
using FluentNHibernate.Mapping;

namespace Infrastructure.Data.NHibernate.Mappings
{
    public class RentalMap : ClassMap<Rental>
    {
        public RentalMap()
        {
            Table("Rentals");
            Id(x => x.Id).GeneratedBy.Identity();
            
            References(x => x.DeliveryDriver)
                .Column("DeliveryDriverId")
                .Not.Nullable();
            
            References(x => x.Motorcycle)
                .Column("MotorcycleId")
                .Not.Nullable();
            
            Map(x => x.RentalPlanDays).Not.Nullable();
            Map(x => x.DailyRate).Not.Nullable();
            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.ExpectedEndDate).Not.Nullable();
            Map(x => x.ActualEndDate).Nullable();
            Map(x => x.TotalCost).Nullable();
            Map(x => x.CreatedAt).Not.Nullable();
        }
    }
}