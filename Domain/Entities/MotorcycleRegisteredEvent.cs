using Domain.Entities;

namespace Domain.Events
{
    public class MotorcycleRegisteredEvent : EntityBase
    {
        public virtual Motorcycle Motorcycle { get; protected set; }
        public virtual string Message { get; protected set; }
        public virtual DateTime EventDate { get; protected set; } = DateTime.UtcNow;
        public virtual bool Is2024Model { get; protected set; }

        protected MotorcycleRegisteredEvent() { }

        public MotorcycleRegisteredEvent(Motorcycle motorcycle, string message)
        {
            Motorcycle = motorcycle ?? throw new ArgumentNullException(nameof(motorcycle));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Is2024Model = motorcycle.Year == 2024;
        }
    }
}