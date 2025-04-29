using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace Application.Services
{
    /// <summary>
    /// Serviço responsável pelas operações relacionadas a entregadores (DeliveryDriver).
    /// </summary>
    public class DeliveryDriverService : IDeliveryDriverService
    {
        private readonly IDeliveryDriverRepository _driverRepository;

        /// <summary>
        /// Construtor que injeta o repositório de entregadores.
        /// </summary>
        /// <param name="driverRepository">Instância do repositório de entregadores.</param>
        public DeliveryDriverService(IDeliveryDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }

        /// <summary>
        /// Registra um novo entregador no sistema após validar seus dados.
        /// </summary>
        /// <param name="identify">Identificador único do entregador.</param>
        /// <param name="fullName">Nome completo do entregador.</param>
        /// <param name="cnpj">CNPJ do entregador.</param>
        /// <param name="birthDate">Data de nascimento do entregador.</param>
        /// <param name="driverLicenseNumber">Número da CNH do entregador.</param>
        /// <param name="driverLicenseType">Tipo da CNH (categoria).</param>
        /// <param name="driverLicenseImagePath">Caminho da imagem da CNH.</param>
        /// <returns>Objeto do tipo <see cref="DeliveryDriver"/> registrado.</returns>
        /// <exception cref="ArgumentException">Lançada se o entregador for menor de idade.</exception>
        /// <exception cref="InvalidOperationException">Lançada se o CNPJ ou o número da CNH já existirem.</exception>
        public async Task<DeliveryDriver> RegisterDriverAsync(
            string identify,
            string fullName,
            string cnpj,
            DateTime birthDate,
            string driverLicenseNumber,
            DriverLicenseType driverLicenseType,
            string driverLicenseImagePath)
        {
            // Verifica se o entregador tem pelo menos 18 anos
            if (DateTime.Now.Year - birthDate.Year < 18)
            {
                throw new ArgumentException("O entregador deve ter pelo menos 18 anos");
            }

            // Verifica se já existe um entregador com o mesmo CNPJ
            var existingByCnpj = await _driverRepository.GetByCnpjAsync(cnpj);
            if (existingByCnpj != null)
            {
                throw new InvalidOperationException("CNPJ já cadastrado");
            }

            // Verifica se já existe um entregador com o mesmo número de CNH
            var existingByLicense = await _driverRepository.GetByDriverLicenseAsync(driverLicenseNumber);
            if (existingByLicense != null)
            {
                throw new InvalidOperationException("Número da CNH já cadastrado");
            }

            // Cria um novo objeto DeliveryDriver
            var driver = new DeliveryDriver
            {
                Identifier = identify,
                FullName = fullName,
                CNPJ = cnpj,
                BirthDate = birthDate,
                DriverLicenseNumber = driverLicenseNumber,
                DriverLicenseType = driverLicenseType,
                DriverLicenseImagePath = driverLicenseImagePath
            };

            // Salva o novo entregador no repositório
            await _driverRepository.AddAsync(driver);
            return driver;
        }

        /// <summary>
        /// Busca um entregador pelo seu ID.
        /// </summary>
        /// <param name="id">Identificador único do entregador.</param>
        /// <returns>Entregador encontrado ou nulo se não existir.</returns>
        public async Task<DeliveryDriver?> GetDriverByIdAsync(Guid id)
        {
            return await _driverRepository.GetByIdAsync(id);
        }
    }
}
