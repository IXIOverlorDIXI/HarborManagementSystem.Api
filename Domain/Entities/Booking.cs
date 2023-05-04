using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Booking : BaseEntity
    {
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public Guid? ShipId { get; set; }
        
        public Guid? BerthId { get; set; }
        
        public virtual Ship Ship { get; set; }
        
        public virtual Berth Berth { get; set; }
        
        public virtual BookingCheck BookingCheck { get; set; }
        
        public virtual ICollection<AdditionalService> AdditionalServices { get; set; }
    }
}