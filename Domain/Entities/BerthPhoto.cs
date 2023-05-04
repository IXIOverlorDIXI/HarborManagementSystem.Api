using System;

namespace Domain.Entities
{
    public class BerthPhoto : BaseEntity
    {
        public Guid BerthId { get; set; }
        
        public Guid PhotoId { get; set; }
        
        public virtual Berth Berth { get; set; }
        
        public virtual File Photo { get; set; }
    }
}