using Domain.Constants;
using Domain.Entities;
using NHibernate;

namespace Infrastructure.Data.Seeders
{
    public static class RoleSeeder
    {
        public static void Seed(ISession session)
        {
            var existingRoles = session.Query<Role>().ToList();
            var rolesToAdd = RoleConstants.AllRoles
                .Where(roleName => !existingRoles.Any(r => r.Name == roleName))
                .Select(roleName => new Role(roleName, $"{roleName} role"));

            foreach (var role in rolesToAdd)
            {
                session.Save(role);
            }
        }
    }
}