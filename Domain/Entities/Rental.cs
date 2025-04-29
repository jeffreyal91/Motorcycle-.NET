using System;

namespace Domain.Entities
{
    public class Rental : EntityBase
    {
        public virtual required DeliveryDriver DeliveryDriver { get; set; }
        public virtual required Motorcycle Motorcycle { get; set; }
        public virtual int RentalPlanDays { get; set; }
        public virtual decimal DailyRate { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime ExpectedEndDate { get; set; }
        public virtual DateTime? ActualEndDate { get; set; }
        public virtual decimal? TotalCost { get; protected set; }
        public virtual DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;


        public virtual void CalculateTotalCost(DateTime returnDate)
        {
            ActualEndDate = returnDate;

            if (returnDate < ExpectedEndDate) // Early return
            {
                var unusedDays = (ExpectedEndDate - returnDate).Days;
                decimal finePercentage = RentalPlanDays switch
                {
                    7 => 0.20m,
                    15 => 0.40m,
                    _ => 0m
                };
                var fine = DailyRate * unusedDays * finePercentage;
                TotalCost = DailyRate * RentalPlanDays - (DailyRate * unusedDays) + fine;
            }
            else if (returnDate > ExpectedEndDate) // Late return
            {
                var extraDays = (returnDate - ExpectedEndDate).Days;
                TotalCost = DailyRate * RentalPlanDays + (50 * extraDays);
            }
            else //Exact return
            {
                TotalCost = DailyRate * RentalPlanDays;
            }
        }
    }
}