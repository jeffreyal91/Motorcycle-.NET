using System.Text.Json.Serialization;
using Domain.Events;

namespace Domain.Entities
{
    public class Motorcycle : EntityBase
    {
        public virtual string Identifier { get;  set; }
        public virtual int Year { get;  set; }
        public virtual string Model { get;  set; }
        public virtual string LicensePlate { get; set; }
        public virtual DateTime CreatedAt { get;  set; } = DateTime.UtcNow;
        public virtual bool Available { get;  set; }
        [JsonIgnore]
        public virtual IList<MotorcycleRegisteredEvent> Events { get; protected set; } = new List<MotorcycleRegisteredEvent>();

        public Motorcycle() { } 

        public Motorcycle(string identifier, int year, string model, string licensePlate, DateTime createdAt, bool available)
        {
            Validate(identifier, year, model, licensePlate);
            Identifier = identifier;
            Year = year;
            Model = model;
            LicensePlate = licensePlate;
            CreatedAt = createdAt;
            Available = available;

            // Generate domain event
            AddEvent(new MotorcycleRegisteredEvent(this, $"New motorcycle registered: {licensePlate}"));
        }

        private void Validate(string identifier, int year, string model, string licensePlate)
        {
            if (year < 1900 || year > DateTime.Now.Year + 1)
                throw new BusinessRuleException("Invalid year");

            if (string.IsNullOrWhiteSpace(model))
                throw new BusinessRuleException("Model is required");

            if (string.IsNullOrWhiteSpace(licensePlate))
                throw new BusinessRuleException("License plate is required");
        }

        protected virtual void AddEvent(MotorcycleRegisteredEvent @event)
        {
            Events.Add(@event);
        }
    }
}