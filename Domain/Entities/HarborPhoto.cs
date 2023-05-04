using System;

namespace Domain.Entities
{
    public class HarborPhoto : BaseEntity
    {
        public Guid HarborId { get; set; }
        
        public Guid PhotoId { get; set; }
        
        public virtual Harbor Harbor { get; set; }
        
        public virtual File Photo { get; set; }
    }
}