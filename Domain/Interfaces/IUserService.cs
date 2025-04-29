using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateUserAsync(string email, string passwordHash);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task AssignRoleAsync(int userId, int roleId);
        Task RemoveRoleAsync(int userId, int roleId);
        Task<bool> VerifyPasswordAsync(int userId, string password);
        Task<bool> UserHasRoleAsync(int userId, string roleName);
    }
}