using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Berth : BaseEntity
    {
        public string DisplayName { get; set; }
        
        public string Description { get; set; }
        
        public double Price { get; set; }
        
        public Guid HarborId { get; set; }
        
        public bool IsActive { get; set; }
        
        public bool IsDeleted { get; set; }
        
        public virtual Harbor Harbor { get; set; }
        
        public virtual ICollection<SuitableShipType> SuitableShipTypes { get; set; }
        
        public virtual ICollection<BerthPhoto> BerthPhotos { get; set; }
        
        public virtual ICollection<Review> Reviews { get; set; }
        
        public virtual ICollection<Booking> Bookings { get; set; }
        
        public virtual ICollection<RelativePositionMetering> RelativePositionMeterings { get; set; }
        
        public virtual ICollection<EnvironmentalCondition> EnvironmentalConditions { get; set; }
        
        public virtual ICollection<StorageEnvironmentalCondition> StorageEnvironmentalConditions { get; set; }
    }
}