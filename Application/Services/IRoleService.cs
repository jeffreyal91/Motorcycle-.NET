using Domain.Entities;
using System.Collections.Generic;

namespace Application.Services
{
     public interface IRoleService
    {
        IEnumerable<Role> GetAllRoles();
        Role GetRoleByName(string name);
        void AssignRoleToUser(int userId, int roleId);
        void RemoveRoleFromUser(int userId, int roleId);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByNameAsync(string name);
        Task AssignRoleToUserAsync(int userId, int roleId);
        Task RemoveRoleFromUserAsync(int userId, int roleId);
    }
}