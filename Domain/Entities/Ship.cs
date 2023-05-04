using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Ship : BaseEntity
    {
        public string DisplayName { get; set; }
        
        public string OwnerId { get; set; }
        
        public Guid? PhotoId { get; set; }
        
        public Guid ShipTypeId { get; set; }
        
        public bool IsDeleted { get; set; }
        
        public virtual AppUser Owner { get; set; }
        
        public virtual File Photo { get; set; }
        
        public virtual ShipType ShipType { get; set; }
        
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}