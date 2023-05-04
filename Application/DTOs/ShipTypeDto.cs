using System;

namespace Application.DTOs
{
    public class ShipTypeDto
    {
        public Guid Id { get; set; }
        
        public string TypeName { get; set; }
        
        public string Description { get; set; }
    }
}