using System;

namespace Application.DTOs
{
    public class StorageEnvironmentalConditionDto
    {
        public Guid Id { get; set; }
        
        public Guid BerthId { get; set; }
        
        public double AirPollution { get; set; }
        
        public double RadiationLevel { get; set; }
        
        public double ShipTemperature { get; set; }
        
        public DateTime MeteringDate { get; set; }
    }
}