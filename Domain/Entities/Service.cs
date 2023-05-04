using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Service : BaseEntity
    {
        public string DisplayName { get; set; }
        
        public string Description { get; set; }
        
        public double Price { get; set; }
        
        public bool IsDeleted { get; set; }
        
        public Guid HarborId { get; set; }
        
        public virtual Harbor Harbor { get; set; }
        
        public virtual ICollection<AdditionalService> AdditionalServices { get; set; }
    }
}