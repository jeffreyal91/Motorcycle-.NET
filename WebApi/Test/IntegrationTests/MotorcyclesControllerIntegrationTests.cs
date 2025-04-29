using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using Domain.Interfaces;
using Domain.Entities;
using Application.Services;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace WebApi.Tests.IntegrationTests
{
    public class MotorcyclesControllerIntegrationTests : IDisposable
    {
        private readonly Mock<IMotorcycleService> _mockService;
        private readonly MotorcyclesController _controller;
        private readonly List<Motorcycle> _testMotorcycles;

        public MotorcyclesControllerIntegrationTests()
        {
            _mockService = new Mock<IMotorcycleService>();
            _controller = new MotorcyclesController(_mockService.Object);

              _testMotorcycles = new List<Motorcycle>();
            
            // Motocicleta 1 - Asignación campo por campo
            var motorcycle1 = new Motorcycle();
            //motorcycle1.Id = 1;
            motorcycle1.Identifier = "M001";
            motorcycle1.Year = 2020;
            motorcycle1.Model = "Honda CB500";
            motorcycle1.LicensePlate = "ABC1234";
            _testMotorcycles.Add(motorcycle1);

            // Motocicleta 2 - Asignación campo por campo
            var motorcycle2 = new Motorcycle();
            //motorcycle2.Id = 2;
            motorcycle2.Identifier = "M002";
            motorcycle2.Year = 2021;
            motorcycle2.Model = "Yamaha MT07";
            motorcycle2.LicensePlate = "XYZ5678";
            _testMotorcycles.Add(motorcycle2);    
        }

        public void Dispose()
        {
            _mockService.Reset();
        }

        [Fact]
        public async Task GetAllMotorcycles_ReturnsAllMotorcycles()
        {
            // Arrange
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(_testMotorcycles);

            // Act
            var result = await _controller.GetAllMotorcycles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var motorcycles = Assert.IsAssignableFrom<IEnumerable<Motorcycle>>(okResult.Value);
            Assert.Equal(2, motorcycles.Count());
        }

        [Fact]
        public async Task GetAllMotorcycles_WithLicensePlate_ReturnsFilteredMotorcycles()
        {
            // Arrange
            var licensePlate = "ABC1234";
            _mockService.Setup(s => s.GetByLicensePlateAsync(licensePlate))
                .ReturnsAsync(_testMotorcycles.Where(m => m.LicensePlate == licensePlate));

            // Act
            var result = await _controller.GetAllMotorcycles(licensePlate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var motorcycles = Assert.IsAssignableFrom<IEnumerable<Motorcycle>>(okResult.Value);
            Assert.Single(motorcycles);
            Assert.Equal(licensePlate, motorcycles.First().LicensePlate);
        }

        [Fact]
        public async Task GetMotorcycle_ExistingId_ReturnsMotorcycle()
        {
            // Arrange
            var testId = 1;
            _mockService.Setup(s => s.GetByIdAsync(testId))
                .ReturnsAsync(_testMotorcycles.First(m => m.Id == testId));

            // Act
            var result = await _controller.GetMotorcycle(testId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var motorcycle = Assert.IsType<Motorcycle>(okResult.Value);
            Assert.Equal(testId, motorcycle.Id);
        }

        [Fact]
        public async Task GetMotorcycle_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var testId = 99;
            _mockService.Setup(s => s.GetByIdAsync(testId)).ReturnsAsync((Motorcycle)null);

            // Act
            var result = await _controller.GetMotorcycle(testId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

       

        [Fact]
        public async Task CreateMotorcycle_DuplicateLicensePlate_ReturnsConflict()
        {
            // Arrange
            var request = new CreateMotorcycleRequest("M004", 2022, "Suzuki GSX", "ABC1234");

            _mockService.Setup(s => s.CreateMotorcycleAsync(
                request.identificador,
                request.ano,
                request.modelo,
                request.placa))
                .ThrowsAsync(new InvalidOperationException("License plate already registered: ABC1234"));

            // Act
            var result = await _controller.CreateMotorcycle(request);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            var error = Assert.IsType<dynamic>(conflictResult.Value);
            Assert.Equal("License plate already registered", error.Error.ToString());
        }

       

        [Fact]
        public async Task UpdateLicensePlate_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var testId = 99;
            var newPlate = "NEW1234";
            var request = new UpdateLicensePlateRequest(newPlate);

            _mockService.Setup(s => s.UpdateLicensePlateAsync(testId, newPlate))
                .ReturnsAsync((Motorcycle)null);

            // Act
            var result = await _controller.UpdateLicensePlate(testId, request);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateLicensePlate_DuplicatePlate_ReturnsConflict()
        {
            // Arrange
            var testId = 1;
            var duplicatePlate = "XYZ5678";
            var request = new UpdateLicensePlateRequest(duplicatePlate);

            _mockService.Setup(s => s.UpdateLicensePlateAsync(testId, duplicatePlate))
                .ThrowsAsync(new InvalidOperationException("License plate already registered: XYZ5678"));

            // Act
            var result = await _controller.UpdateLicensePlate(testId, request);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            var error = Assert.IsType<dynamic>(conflictResult.Value);
            Assert.Equal("License plate already registered", error.Error.ToString());
        }

        [Fact]
        public async Task DeleteMotorcycle_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var testId = 1;
            _mockService.Setup(s => s.DeleteMotorcycleAsync(testId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteMotorcycle(testId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteMotorcycle_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var testId = 99;
            _mockService.Setup(s => s.DeleteMotorcycleAsync(testId))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.DeleteMotorcycle(testId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteMotorcycle_WithExistingRentals_ReturnsBadRequest()
        {
            // Arrange
            var testId = 1;
            _mockService.Setup(s => s.DeleteMotorcycleAsync(testId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteMotorcycle(testId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<dynamic>(badRequestResult.Value);
            Assert.Equal("Cannot delete motorcycle with existing rentals", error.Error.ToString());
        }

        [Fact]
        public void CreateMotorcycleRequest_Validation_ValidModel_Passes()
        {
            // Arrange
            var request = new CreateMotorcycleRequest("M005", 2023, "BMW S1000", "GHI3456");

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Theory]
        [InlineData(null, 2023, "Model", "PLATE")] // Identificador faltante
        [InlineData("M006", 0, "Model", "PLATE")]   // Año inválido
        [InlineData("M007", 2023, null, "PLATE")]    // Modelo faltante
        [InlineData("M008", 2023, "Model", null)]      // Placa faltante
        public void CreateMotorcycleRequest_Validation_InvalidModel_Fails(string identificador, int ano, string modelo, string placa)
        {
            // Arrange
            var request = new CreateMotorcycleRequest(identificador, ano, modelo, placa);

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(validationResults);
        }
    }
}