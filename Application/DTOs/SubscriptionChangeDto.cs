using System;

namespace Application.DTOs
{
    public class SubscriptionChangeDto
    {
        public double ChangeCost { get; set; }
        
        public SubscriptionDto NewSubscription { get; set; }
    }
}