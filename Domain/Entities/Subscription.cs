using System.Collections.Generic;

namespace Domain.Entities
{
    public class Subscription : BaseEntity
    {
        public string DisplayName { get; set; }
        
        public string Description { get; set; }
        
        public int MaxHarborAmount { get; set; }
        
        public double TaxOnBooking { get; set; }
        
        public double TaxOnServices { get; set; }
        
        public double Price { get; set; }
        
        public bool IsDeleted { get; set; }
        
        public virtual ICollection<AppUser> Users { get; set; }
        
        public virtual ICollection<SubscriptionСheck> SubscriptionСhecks { get; set; }
    }
}