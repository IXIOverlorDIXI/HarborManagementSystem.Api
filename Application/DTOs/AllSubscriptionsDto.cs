using System.Collections.Generic;

namespace Application.DTOs
{
    public class AllSubscriptionsDto
    {
        public List<SubscriptionDto> Subscriptions { get; set; }
        
        public int CurrentSubscriptionIndex { get; set; }
    }
}