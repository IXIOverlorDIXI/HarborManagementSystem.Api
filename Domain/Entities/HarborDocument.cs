using System;

namespace Domain.Entities
{
    public class HarborDocument : BaseEntity
    {
        public Guid HarborId { get; set; }
        
        public Guid DocumentId { get; set; }
        
        public bool IsDeleted { get; set; }
        
        public DateTime DateOfUpload { get; set; }
        
        public DateTime? DateOfDelete { get; set; }
        
        public virtual Harbor Harbor { get; set; }
        
        public virtual File Document { get; set; }
    }
}