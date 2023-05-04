using System;

namespace Domain.Entities
{
    public class SuitableShipType : BaseEntity
    {
        public Guid BerthId { get; set; }
        
        public Guid ShipTypeId { get; set; }
        
        public virtual Berth Berth { get; set; }
        
        public virtual ShipType ShipType { get; set; }
    }
}