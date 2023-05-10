using System;

namespace Domain.Entities
{
    public class RelativePositionMetering : BaseEntity
    {
        public double LeftDistance { get; set; }
        
        public double RightDistance { get; set; }
        
        public double FrontDistance { get; set; }
        
        public double BackDistance { get; set; }
        
        public double RotationAngle { get; set; }
        
        public double TiltAngle { get; set; }
        
        public double RollAngle { get; set; }
        
        public double HeightHeadAboveWater { get; set; }
        
        public double HeightTailAboveWater { get; set; }
        
        public DateTime MeteringDate { get; set; }
        
        public Guid BerthId { get; set; }
        
        public virtual Berth Berth { get; set; }
    }
}