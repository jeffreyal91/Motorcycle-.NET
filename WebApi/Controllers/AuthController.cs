using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Controlador responsável pela autenticação de usuários.
/// Gera tokens JWT baseados nas credenciais fornecidas.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Construtor que injeta a configuração da aplicação.
    /// </summary>
    /// <param name="configuration">Instância da configuração.</param>
    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Endpoint para autenticar o usuário e gerar um token JWT.
    /// </summary>
    /// <param name="model">Modelo contendo o nome de usuário e senha.</param>
    /// <returns>Token JWT se a autenticação for bem-sucedida; caso contrário, retorna Unauthorized.</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        // Verifica se o usuário é "admin" com senha correta
        if (model.Username == "admin" && model.Password == "password")
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.Role, "Admin") // Atribui papel de Administrador
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1), // Token expira em 1 dia
                signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
        // Verifica se o usuário é "user" com senha correta
        else if (model.Username == "user" && model.Password == "password")
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.Role, "DeliveryDriver") // Atribui papel de Entregador
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        // Retorna 401 Unauthorized se as credenciais não forem válidas
        return Unauthorized();
    }
}

/// <summary>
/// Modelo de dados para login do usuário.
/// Contém o nome de usuário e senha.
/// </summary>
public class LoginModel
{
    /// <summary>
    /// Nome de usuário.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    public string Password { get; set; }
}
