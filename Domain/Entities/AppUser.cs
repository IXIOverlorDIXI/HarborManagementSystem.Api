using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        
        public string Settings { get; set; }
        
        public Guid? PhotoId { get; set; }
        
        public Guid? SubscriptionId { get; set; }

        public virtual File Photo { get; set; }
        
        public virtual Subscription Subscription { get; set; }
        
        public virtual ICollection<Harbor> Harbors { get; set; }
        
        public virtual ICollection<Review> Reviews { get; set; }
        
        public virtual ICollection<SubscriptionСheck> SubscriptionСhecks { get; set; }
        
        public virtual ICollection<Ship> Ships { get; set; }
    }
}