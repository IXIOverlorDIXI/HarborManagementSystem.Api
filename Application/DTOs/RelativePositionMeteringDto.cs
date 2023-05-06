using System;
using Newtonsoft.Json;

namespace Application.DTOs
{
    public class RelativePositionMeteringDto
    {
        [JsonProperty("Id")]
        public Guid Id { get; set; }
        
        [JsonProperty("LeftDistance")]
        public double LeftDistance { get; set; }
        
        [JsonProperty("RightDistance")]
        public double RightDistance { get; set; }
        
        [JsonProperty("FrontDistance")]
        public double FrontDistance { get; set; }
        
        [JsonProperty("BackDistance")]
        public double BackDistance { get; set; }
        
        [JsonProperty("RotationAngle")]
        public double RotationAngle { get; set; }
        
        [JsonProperty("TiltAngle")]
        public double TiltAngle { get; set; }
        
        [JsonProperty("HeightHeadAboveWater")]
        public double HeightHeadAboveWater { get; set; }
        
        [JsonProperty("HeightTailAboveWater")]
        public double HeightTailAboveWater { get; set; }
        
        [JsonProperty("BerthId")]
        public Guid BerthId { get; set; }
        
        [JsonProperty("MeteringDate")]
        public DateTime MeteringDate { get; set; }
    }
}