using Application.Services;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controlador responsável pela gestão de papéis (roles) dos usuários.
/// Permite listar, atribuir e remover papéis.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IRoleService _roleService;

    /// <summary>
    /// Construtor que injeta o serviço de gestão de papéis.
    /// </summary>
    /// <param name="roleService">Serviço de papéis de usuário.</param>
    public UsersController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Retorna todos os papéis (roles) disponíveis no sistema.
    /// </summary>
    /// <returns>Lista de papéis.</returns>
    [HttpGet("roles")]
    public IActionResult GetAllRoles()
    {
        return Ok(_roleService.GetAllRoles());
    }

    /// <summary>
    /// Atribui um papel (role) a um usuário.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="roleId">ID do papel a ser atribuído.</param>
    /// <returns>Status 204 No Content se sucesso.</returns>
    [HttpPost("{userId}/roles/{roleId}")]
    public IActionResult AssignRole(int userId, int roleId)
    {
        _roleService.AssignRoleToUser(userId, roleId);
        return NoContent();
    }

    /// <summary>
    /// Remove um papel (role) de um usuário.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="roleId">ID do papel a ser removido.</param>
    /// <returns>Status 204 No Content se sucesso.</returns>
    [HttpDelete("{userId}/roles/{roleId}")]
    public IActionResult RemoveRole(int userId, int roleId)
    {
        _roleService.RemoveRoleFromUser(userId, roleId);
        return NoContent();
    }
}
