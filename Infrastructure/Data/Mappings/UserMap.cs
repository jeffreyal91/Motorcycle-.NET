using Domain.Entities;
using FluentNHibernate.Mapping;

namespace Infrastructure.Data.Mappings
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("users");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Email).Not.Nullable().Length(100).Unique();
            Map(x => x.PasswordHash).Not.Nullable().Length(255);
            HasManyToMany(x => x.Roles)
                .Table("user_roles")
                .ParentKeyColumn("user_id")
                .ChildKeyColumn("role_id")
                .Cascade.SaveUpdate();
        }
    }
}