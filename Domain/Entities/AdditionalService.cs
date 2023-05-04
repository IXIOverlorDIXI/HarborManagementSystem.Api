using System;

namespace Domain.Entities
{
    public class AdditionalService : BaseEntity
    {
        public Guid ServiceId { get; set; }
        
        public Guid BookingId { get; set; }
        
        public virtual Service Service { get; set; }
        
        public virtual Booking Booking { get; set; }
    }
}