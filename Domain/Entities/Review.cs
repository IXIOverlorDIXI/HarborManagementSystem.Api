using System;

namespace Domain.Entities
{
    public class Review : BaseEntity
    {
        public int ReviewMark { get; set; }
        
        public string ReviewBody { get; set; }
        
        public string ReviewPluses { get; set; }
        
        public string ReviewMinuses { get; set; }
        
        public DateTime Date { get; set; }
        
        public Guid BerthId { get; set; }
        
        public string ReviewerId { get; set; }
        
        public virtual Berth Berth { get; set; }
        
        public virtual AppUser Reviewer { get; set; }
    }
}