using Domain.Events;
using FluentNHibernate.Mapping;

namespace Infrastructure.Data.Mappings
{
    public class MotorcycleEventMap : ClassMap<MotorcycleRegisteredEvent>
    {
        public MotorcycleEventMap()
        {
            Table("motorcycle_events");
            Id(x => x.Id).GeneratedBy.Identity();
            //References(x => x.Motorcycle).Column("motorcycle_id").Not.Nullable();
            Map(x => x.Message).Not.Nullable().Length(500);
            Map(x => x.EventDate).Not.Nullable();
            Map(x => x.Is2024Model).Not.Nullable();
            
            
        }
    }
}