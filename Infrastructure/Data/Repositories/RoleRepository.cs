// Infrastructure/Data/Repositories/RoleRepository.cs
using Domain.Entities;
using Domain.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(ISession session) : base(session)
        {
        }

        public async Task<Role> GetByNameAsync(string name)
        {
            return await _session.Query<Role>()
                .FirstOrDefaultAsync(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}