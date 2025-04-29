using Domain.Entities;

namespace Domain.Events
{
    public class MotorcycleRegistered2024Event
    {
        public Motorcycle Motorcycle { get; }
        public string Message { get; }

        public MotorcycleRegistered2024Event(Motorcycle motorcycle, string message)
        {
            Motorcycle = motorcycle;
            Message = message;
        }
    }
}