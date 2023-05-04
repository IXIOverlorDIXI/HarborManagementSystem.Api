using System;

namespace Domain.Entities
{
    public class SubscriptionСheck : BaseEntity
    {
        public string BankTransactionId { get; set; }
        
        public string Description { get; set; }
        
        public DateTime Date { get; set; }
        
        public double TotalCost { get; set; }
        
        public string UserId { get; set; }
        
        public Guid SubscriptionId { get; set; }
        
        public virtual AppUser AppUser { get; set; }
        
        public virtual Subscription Subscription { get; set; }
    }
}