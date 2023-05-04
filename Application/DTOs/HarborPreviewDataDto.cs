using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace Application.DTOs
{
    public class HarborPreviewDataDto
    {
        public Guid Id { get; set; }
    
        public string DisplayName { get; set; }
        
        public string Description { get; set; }
        
        public string SupportEmail { get; set; }
        
        public string SupportPhoneNumber { get; set; }

        public double GeolocationLongitude { get; set; }
    
        public double GeolocationLatitude  { get; set; }
        
        public List<string> Photos { get; set; }
        
        public double AverageRate { get; set; }
        
        public int ReviewsAmount { get; set; }
    }
}