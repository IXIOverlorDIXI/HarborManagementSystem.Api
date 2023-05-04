using System;

namespace Application.DTOs
{
    public class ShipDataDto
    {
        public Guid Id { get; set; }
        
        public string DisplayName { get; set; }

        public Guid ShipTypeId { get; set; }
    }
}