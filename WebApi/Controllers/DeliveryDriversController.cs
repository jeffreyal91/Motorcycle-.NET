using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controlador responsável pelas operações relacionadas aos entregadores.
    /// Somente usuários autenticados com a política "DeliveryDriverOnly" podem acessar este controlador.
    /// </summary>
    [ApiController]
    [Authorize(Policy = "DeliveryDriverOnly")]
    [Route("entregadores")]
    public class DeliveryDriversController : ControllerBase
    {
        private readonly IDeliveryDriverService _driverService;

        /// <summary>
        /// Construtor que injeta a dependência do serviço de entregadores.
        /// </summary>
        /// <param name="driverService">Serviço que gerencia a lógica de negócios para entregadores.</param>
        public DeliveryDriversController(IDeliveryDriverService driverService)
        {
            _driverService = driverService;
        }

        /// <summary>
        /// Registra um novo entregador no sistema.
        /// </summary>
        /// <param name="request">Objeto contendo as informações necessárias para registrar um entregador.</param>
        /// <returns>Retorna o entregador criado, erro de validação, conflito ou erro interno.</returns>
        [HttpPost]
        public async Task<IActionResult> RegisterDriver([FromBody] RegisterDriverRequest request)
        {
            try
            {
                // Verifica se o modelo recebido é válido
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Valida o tipo de CNH recebido
                if (!Enum.TryParse<DriverLicenseType>(request.tipo_cnh, out var licenseType) ||
                    !Enum.IsDefined(typeof(DriverLicenseType), licenseType))
                {
                    // Retorna erro informando os valores válidos para o tipo de CNH
                    var validValues = string.Join(", ", Enum.GetNames(typeof(DriverLicenseType)));
                    return BadRequest(new
                    {
                        Error = "Tipo de CNH inválido",
                        ValidValues = validValues,
                        ReceivedValue = request.tipo_cnh
                    });
                }

                // Chama o serviço para registrar o novo entregador
                var driver = await _driverService.RegisterDriverAsync(
                    request.identificador,
                    request.nome,
                    request.cnpj,
                    request.data_nascimento,
                    request.numero_cnh,
                    licenseType,
                    request.imagem_cnh);

                // Retorna o entregador criado, com status 201 Created
                return CreatedAtAction(nameof(GetDriver), new { id = driver.Id }, driver);
            }
            catch (ArgumentException ex)
            {
                // Retorna erro 400 Bad Request se houver problema de argumento
                return BadRequest(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                // Retorna erro 409 Conflict se o entregador já existir
                return Conflict(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                // Retorna erro 500 Internal Server Error para qualquer outro erro não tratado
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Busca um entregador pelo seu ID.
        /// </summary>
        /// <param name="id">Identificador único do entregador.</param>
        /// <returns>Retorna o entregador encontrado ou erro 404 se não encontrado.</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetDriver(Guid id)
        {
            try
            {
                // Busca o entregador pelo ID
                var driver = await _driverService.GetDriverByIdAsync(id);

                // Retorna 404 Not Found se não existir ou 200 OK com os dados do entregador
                return driver == null ? NotFound() : Ok(driver);
            }
            catch (Exception ex)
            {
                // Retorna erro 500 Internal Server Error para erros inesperados
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }
    }

    /// <summary>
    /// Estrutura que representa o payload de requisição para registrar um entregador.
    /// </summary>
    public record RegisterDriverRequest(
        [Required] string identificador,            // Identificador único do entregador
        [Required][StringLength(100)] string nome,   // Nome completo do entregador
        [Required][StringLength(14)] string cnpj,    // Número do CNPJ (Cadastro Nacional de Pessoa Jurídica)
        [Required] DateTime data_nascimento,         // Data de nascimento do entregador
        [Required][StringLength(20)] string numero_cnh, // Número da carteira de motorista (CNH)
        [Required] string tipo_cnh,                  // Tipo de CNH (Categoria)
        [Required] string imagem_cnh);               // Imagem (em base64 ou URL) da CNH
}
