using Domain.Entities; 
using Domain.Interfaces; 
using Newtonsoft.Json;    
using System;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public class MotorcycleEventConsumer
    {
        private readonly IMotorcycleService _motorcycleService;

        public MotorcycleEventConsumer(IMotorcycleService motorcycleService)
        {
            _motorcycleService = motorcycleService;
        }


        public async Task ConsumeAsync(string message)
        {
            Console.WriteLine($"Message received: {message}");

            var motorcycle = ParseMessage(message);
            if (motorcycle != null)
            {
                await _motorcycleService.AddMotorcycle(motorcycle);
                Console.WriteLine("Motorcycle processed and added.");
            }
            else
            {
                Console.WriteLine("Error processing message, invalid motorcycle.");
            }
        }

        private Motorcycle ParseMessage(string message)
        {
            try
            {
                var motorcycle = JsonConvert.DeserializeObject<Motorcycle>(message);
                return motorcycle;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing message: {ex.Message}");
                return null;
            }
        }
    }
}
