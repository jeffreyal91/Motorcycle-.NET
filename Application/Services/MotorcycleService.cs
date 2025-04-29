using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using Infrastructure.Messaging;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    /// <summary>
    /// Serviço responsável pelas operações relacionadas a motocicletas.
    /// </summary>
    public class MotorcycleService : IMotorcycleService
    {
        private readonly IMotorcycleRepository _motorcycleRepository;
        private readonly IRentalRepository _rentalRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IMessageBroker _messageBroker;

        /// <summary>
        /// Construtor que injeta as dependências necessárias.
        /// </summary>
        public MotorcycleService(
            IMotorcycleRepository motorcycleRepository,
            IEventRepository eventRepository,
            IMessageBroker messageBroker,
            IRentalRepository rentalRepository)
        {
            _motorcycleRepository = motorcycleRepository ?? throw new ArgumentNullException(nameof(motorcycleRepository));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _messageBroker = messageBroker ?? throw new ArgumentNullException(nameof(messageBroker));
            _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
        }

        /// <summary>
        /// Obtém uma motocicleta pelo seu identificador.
        /// </summary>
        public async Task<Motorcycle> GetByIdAsync(int id)
        {
            return await _motorcycleRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Retorna todas as motocicletas cadastradas.
        /// </summary>
        public async Task<IEnumerable<Motorcycle>> GetAllAsync()
        {
            var motorcycles = await _motorcycleRepository.GetAllAsync();
            return motorcycles.ToList();
        }

        /// <summary>
        /// Verifica se existe uma motocicleta com o ID especificado.
        /// </summary>
        public async Task<bool> ExistsAsync(int id)
        {
            var motorcycle = await _motorcycleRepository.GetByIdAsync(id);
            return motorcycle != null;
        }

        /// <summary>
        /// Verifica se existem motocicletas de um determinado ano.
        /// </summary>
        public async Task<bool> HasMotorcyclesFromYearAsync(int year)
        {
            var motorcycles = await _motorcycleRepository.GetAllAsync();
            return motorcycles.Any(m => m.Year == year);
        }

        /// <summary>
        /// Cria e registra uma nova motocicleta no sistema.
        /// </summary>
        /// <exception cref="ArgumentException">Lançada se modelo, placa ou ano forem inválidos.</exception>
        /// <exception cref="InvalidOperationException">Lançada se a placa já estiver cadastrada.</exception>
        public async Task<Motorcycle> CreateMotorcycleAsync(string identificador, int year, string model, string licensePlate)
        {
            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("O modelo não pode ser vazio", nameof(model));

            if (string.IsNullOrWhiteSpace(licensePlate))
                throw new ArgumentException("A placa não pode ser vazia", nameof(licensePlate));

            if (year < 1900 || year > DateTime.Now.Year + 1)
                throw new ArgumentException($"O ano deve estar entre 1900 e {DateTime.Now.Year + 1}", nameof(year));

            var normalizedPlate = licensePlate.Trim().ToUpper();

            if (await _motorcycleRepository.ExistsByLicensePlateAsync(normalizedPlate))
            {
                throw new InvalidOperationException($"Placa já cadastrada '{normalizedPlate}'");
            }

            var motorcycle = new Motorcycle(identificador, year, model.Trim(), normalizedPlate, DateTime.UtcNow, true);
            await _motorcycleRepository.AddAsync(motorcycle);

            var message = $"Nova motocicleta cadastrada: {identificador} {model} {year}";

            if (year == 2024)
            {
                var motorcycleEvent = new MotorcycleRegisteredEvent(motorcycle, message);
                await _eventRepository.AddAsync(motorcycleEvent);

                await _messageBroker.PublishAsync("motorcycle_events", new
                {
                    EventType = "New2024Motorcycle",
                    EventId = motorcycleEvent.Id,
                    LicensePlate = motorcycle.LicensePlate,
                    Model = motorcycle.Model,
                    Year = motorcycle.Year,
                    CreatedAt = DateTime.UtcNow
                });
            }

            return motorcycle;
        }

        /// <summary>
        /// Busca motocicletas pela placa (suporta busca parcial).
        /// </summary>
        public async Task<IEnumerable<Motorcycle>> GetByLicensePlateAsync(string licensePlate)
        {
            if (string.IsNullOrWhiteSpace(licensePlate))
            {
                throw new ArgumentException("A placa não pode ser vazia", nameof(licensePlate));
            }

            var normalizedPlate = licensePlate.Trim().ToUpper();
            var motorcycles = await _motorcycleRepository.GetAllAsync();

            return motorcycles.Where(m =>
                m.LicensePlate != null &&
                m.LicensePlate.Trim().ToUpper().Contains(normalizedPlate));
        }

        /// <summary>
        /// Adiciona uma nova motocicleta ao repositório.
        /// </summary>
        public Task AddMotorcycle(Motorcycle motorcycle)
        {
            return _motorcycleRepository.AddAsync(motorcycle);
        }

        /// <summary>
        /// Atualiza a placa de uma motocicleta existente.
        /// </summary>
        /// <returns>Motocicleta atualizada ou null se não encontrada.</returns>
        /// <exception cref="ArgumentException">Lançada se a nova placa for inválida.</exception>
        /// <exception cref="InvalidOperationException">Lançada se a nova placa já estiver em uso por outra motocicleta.</exception>
        public async Task<Motorcycle?> UpdateLicensePlateAsync(int id, string newLicensePlate)
        {
            if (string.IsNullOrWhiteSpace(newLicensePlate))
            {
                throw new ArgumentException("A nova placa não pode ser vazia", nameof(newLicensePlate));
            }

            var existingWithPlate = await _motorcycleRepository.GetByLicensePlateAsync(newLicensePlate);

            if (existingWithPlate != null && existingWithPlate.Id != id)
            {
                throw new InvalidOperationException("Placa já cadastrada");
            }

            var motorcycle = await _motorcycleRepository.GetByIdAsync(id);
            if (motorcycle == null)
            {
                return null;
            }

            motorcycle.LicensePlate = newLicensePlate;
            await _motorcycleRepository.UpdateAsync(motorcycle);

            return motorcycle;
        }

        /// <summary>
        /// Exclui uma motocicleta, desde que não esteja vinculada a aluguéis.
        /// </summary>
        /// <returns>Verdadeiro se a motocicleta foi excluída, falso se existem aluguéis associados.</returns>
        public async Task<bool> DeleteMotorcycleAsync(int id)
        {
            if (await _rentalRepository.HasRentalsForMotorcycleAsync(id))
            {
                return false;
            }

            var motorcycle = await _motorcycleRepository.GetByIdAsync(id);
            await _motorcycleRepository.DeleteAsync(motorcycle);
            return true;
        }
    }
}
