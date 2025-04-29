using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controlador responsável por gerenciar as motocicletas.
    /// Apenas usuários com a política "AdminOnly" têm acesso a este controlador.
    /// </summary>
    [ApiController]
    [Route("/moto")]
    [Authorize(Policy = "AdminOnly")]
    public class MotorcyclesController : ControllerBase
    {
        private readonly Domain.Interfaces.IMotorcycleService _motorcycleService;

        /// <summary>
        /// Construtor que injeta o serviço de motocicletas.
        /// </summary>
        /// <param name="motorcycleService">Serviço responsável pelas operações com motocicletas.</param>
        public MotorcyclesController(Domain.Interfaces.IMotorcycleService motorcycleService)
        {
            _motorcycleService = motorcycleService;
        }

        /// <summary>
        /// Recupera todas as motocicletas ou filtra por placa, se informado.
        /// </summary>
        /// <param name="licensePlate">Placa da motocicleta para filtro (opcional).</param>
        /// <returns>Lista de motocicletas.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllMotorcycles([FromQuery] string? licensePlate = null)
        {
            try
            {
                IEnumerable<Motorcycle> motorcycles;

                if (!string.IsNullOrEmpty(licensePlate))
                {
                    motorcycles = await _motorcycleService.GetByLicensePlateAsync(licensePlate);
                }
                else
                {
                    motorcycles = await _motorcycleService.GetAllAsync();
                }

                return Ok(motorcycles);
            }
            catch (Exception)
            {
                // Retorna erro 500 em caso de exceção não tratada
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Recupera uma motocicleta pelo seu ID.
        /// </summary>
        /// <param name="id">Identificador da motocicleta.</param>
        /// <returns>Motocicleta correspondente ou 404 se não encontrada.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMotorcycle(int id)
        {
            try
            {
                var motorcycle = await _motorcycleService.GetByIdAsync(id);

                if (motorcycle == null)
                {
                    return NotFound();
                }

                return Ok(motorcycle);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Cria uma nova motocicleta.
        /// </summary>
        /// <param name="request">Objeto com os dados para criação da motocicleta.</param>
        /// <returns>Motocicleta criada ou erro de validação/conflito.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateMotorcycle([FromBody] CreateMotorcycleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var motorcycle = await _motorcycleService.CreateMotorcycleAsync(
                    request.identificador,
                    request.ano,
                    request.modelo,
                    request.placa);

                return CreatedAtAction(nameof(GetMotorcycle),
                    new { id = motorcycle.Id },
                    motorcycle);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.StartsWith("License plate already registered"))
            {
                // Conflito: placa já cadastrada
                return Conflict(new
                {
                    Error = "License plate already registered",
                    Details = ex.Message,
                    LicensePlate = request.placa
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Atualiza a placa de uma motocicleta existente.
        /// </summary>
        /// <param name="id">Identificador da motocicleta.</param>
        /// <param name="request">Objeto contendo a nova placa.</param>
        /// <returns>Motocicleta atualizada ou erro.</returns>
        [HttpPut("{id}/placa")]
        public async Task<IActionResult> UpdateLicensePlate(int id, [FromBody] UpdateLicensePlateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedMotorcycle = await _motorcycleService.UpdateLicensePlateAsync(id, request.placa);

                if (updatedMotorcycle == null)
                {
                    return NotFound();
                }

                return Ok(updatedMotorcycle);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.StartsWith("License plate already registered"))
            {
                return Conflict(new
                {
                    Error = "License plate already registered",
                    Details = ex.Message,
                    LicensePlate = request.placa
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Deleta uma motocicleta existente.
        /// </summary>
        /// <param name="id">Identificador da motocicleta.</param>
        /// <returns>Status 204 No Content se sucesso, 400 ou 404 em caso de erro.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMotorcycle(int id)
        {
            try
            {
                var result = await _motorcycleService.DeleteMotorcycleAsync(id);

                if (!result)
                {
                    return BadRequest(new { Error = "Cannot delete motorcycle with existing rentals" });
                }

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }
    }

    /// <summary>
    /// Representa a estrutura da requisição para criação de uma motocicleta.
    /// </summary>
    /// <param name="identificador">Identificador único da motocicleta.</param>
    /// <param name="ano">Ano de fabricação da motocicleta.</param>
    /// <param name="modelo">Modelo da motocicleta.</param>
    /// <param name="placa">Placa da motocicleta.</param>
    public record CreateMotorcycleRequest(string identificador, int ano, string modelo, string placa);

    /// <summary>
    /// Representa a estrutura da requisição para atualização da placa de uma motocicleta.
    /// </summary>
    /// <param name="placa">Nova placa da motocicleta.</param>
    public record UpdateLicensePlateRequest(string placa);
}
