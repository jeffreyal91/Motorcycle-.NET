using Domain.Entities;

namespace Domain.Services
{
    public static class RentalValidationService
    {
        public static void ValidateRental(Rental rental)
        {
            // Validate kind of license
            if (!rental.DeliveryDriver.CanRentMotorcycles())
            {
                throw new BusinessRuleException("Driver license type must be A or A+B to rent motorcycles");
            }

            // Validate start day (at least tomorrow)
            if (rental.StartDate.Date <= DateTime.UtcNow.Date)
            {
                throw new BusinessRuleException("Start date must be at least tomorrow");
            }

            // Validate rental plan
            var validPlans = new[] { 7, 15, 30, 45, 50 };
            if (!validPlans.Contains(rental.RentalPlanDays))
            {
                throw new BusinessRuleException("Invalid rental plan days");
            }
        }
    }

}