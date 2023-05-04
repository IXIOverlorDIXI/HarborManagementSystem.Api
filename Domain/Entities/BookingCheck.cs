using System;

namespace Domain.Entities
{
    public class BookingCheck : BaseEntity
    {
        public string BankTransactionId { get; set; }
        
        public string Description { get; set; }
        
        public DateTime Date { get; set; }
        
        public double TotalCost { get; set; }
        
        public Guid BookingId { get; set; }
        
        public virtual Booking Booking { get; set; }
    }
}