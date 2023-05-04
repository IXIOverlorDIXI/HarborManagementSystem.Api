using System;

namespace Application.DTOs
{
    public class SuitableBerthSearchModel
    {
        public ShipTypeDto ShipType { get; set; }
        
        public Guid HarborId { get; set; }
    }
}