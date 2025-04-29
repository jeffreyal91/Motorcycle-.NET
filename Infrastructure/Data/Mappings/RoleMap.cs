using Domain.Entities;
using FluentNHibernate.Mapping;

namespace Infrastructure.Data.Mappings
{
    public class RoleMap : ClassMap<Role>
    {
        public RoleMap()
        {
            Table("roles");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name).Not.Nullable().Length(50);
            Map(x => x.Description).Length(255);
            HasManyToMany(x => x.Users)
                .Table("user_roles")
                .ParentKeyColumn("role_id")
                .ChildKeyColumn("user_id")
                .Inverse()
                .Cascade.None();
        }
    }
}