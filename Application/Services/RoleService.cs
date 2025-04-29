using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    /// <summary>
    /// Serviço responsável pelo gerenciamento de papéis (roles) de usuários.
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<User> _userRepository;

        /// <summary>
        /// Construtor que injeta os repositórios necessários.
        /// </summary>
        public RoleService(IRepository<Role> roleRepository, IRepository<User> userRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Retorna todos os papéis cadastrados.
        /// </summary>
        public IEnumerable<Role> GetAllRoles()
        {
            return _roleRepository.GetAll().ToList();
        }

        /// <summary>
        /// Busca um papel pelo nome.
        /// </summary>
        /// <param name="name">Nome do papel a ser buscado.</param>
        /// <returns>Instância de <see cref="Role"/> ou null se não encontrado.</returns>
        public Role GetRoleByName(string name)
        {
            return _roleRepository.GetAll()
                .FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Atribui um papel a um usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="roleId">Identificador do papel.</param>
        public void AssignRoleToUser(int userId, int roleId)
        {
            var user = _userRepository.GetById(userId);
            var role = _roleRepository.GetById(roleId);

            if (user == null || role == null) return;

            user.AssignRole(role);
            _userRepository.Update(user);
        }

        /// <summary>
        /// Remove um papel atribuído a um usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="roleId">Identificador do papel.</param>
        public void RemoveRoleFromUser(int userId, int roleId)
        {
            var user = _userRepository.GetById(userId);
            var role = _roleRepository.GetById(roleId);

            if (user == null || role == null) return;

            user.RemoveRole(role);
            _userRepository.Update(user);
        }

        /// <summary>
        /// Retorna todos os papéis cadastrados de forma assíncrona.
        /// </summary>
        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return (await _roleRepository.GetAllAsync()).ToList();
        }

        /// <summary>
        /// Busca um papel pelo nome de forma assíncrona.
        /// </summary>
        /// <param name="name">Nome do papel a ser buscado.</param>
        /// <returns>Instância de <see cref="Role"/> ou null se não encontrado.</returns>
        public async Task<Role> GetRoleByNameAsync(string name)
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Atribui um papel a um usuário de forma assíncrona.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="roleId">Identificador do papel.</param>
        public async Task AssignRoleToUserAsync(int userId, int roleId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var role = await _roleRepository.GetByIdAsync(roleId);

            if (user == null || role == null) return;

            user.AssignRole(role);
            await _userRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Remove um papel atribuído a um usuário de forma assíncrona.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="roleId">Identificador do papel.</param>
        public async Task RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var role = await _roleRepository.GetByIdAsync(roleId);

            if (user == null || role == null) return;

            user.RemoveRole(role);
            await _userRepository.UpdateAsync(user);
        }
    }
}
