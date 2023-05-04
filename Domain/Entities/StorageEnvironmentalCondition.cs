using System;

namespace Domain.Entities
{
    public class StorageEnvironmentalCondition : BaseEntity
    {
        public double AirPollution { get; set; }
        
        public double RadiationLevel { get; set; }
        
        public double ShipTemperature { get; set; }
        
        public DateTime MeteringDate { get; set; }
        
        public Guid BerthId { get; set; }
        
        public virtual Berth Berth { get; set; }
    }
}