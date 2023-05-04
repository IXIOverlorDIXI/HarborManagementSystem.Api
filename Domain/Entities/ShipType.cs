using System.Collections.Generic;

namespace Domain.Entities
{
    public class ShipType : BaseEntity
    { 
        public string TypeName { get; set; }
        
        public string Description { get; set; }

        public virtual ICollection<Ship> Ships { get; set; }
        
        public virtual ICollection<SuitableShipType> SuitableShipTypes { get; set; }
    }
}