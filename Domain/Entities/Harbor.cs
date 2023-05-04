using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Harbor : BaseEntity
    {
        public string DisplayName { get; set; }
        
        public string Description { get; set; }
        
        public string SupportEmail { get; set; }
        
        public string SupportPhoneNumber { get; set; }
        
        public string BIC { get; set; }
        
        public string IBAN { get; set; }
        
        public Point Geolocation { get; set; }
        
        public string OwnerId { get; set; }
        
        public bool IsDeleted { get; set; }
        
        public virtual AppUser Owner { get; set; }
        
        public virtual ICollection<Berth> Berths { get; set; }
        
        public virtual ICollection<HarborPhoto> HarborPhotos { get; set; }
        
        public virtual ICollection<HarborDocument> HarborDocuments { get; set; }
        
        public virtual ICollection<Service> Services { get; set; }
    }
}