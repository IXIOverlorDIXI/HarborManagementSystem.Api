using System.Collections.Generic;

namespace Domain.Entities
{
    public class File : BaseEntity
    {
        public string Url { get; set; }

        public virtual AppUser AppUser { get; set; }
        
        public virtual Ship Ship { get; set; }
        
        public virtual BerthPhoto BerthPhoto { get; set; }
        
        public virtual HarborDocument HarborDocument { get; set; }
        
        public virtual HarborPhoto HarborPhoto { get; set; }
    }
}