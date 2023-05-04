using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.DTOs;
using Application.SubscriptionCheck;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class SubscriptionCheckController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllSubscriptions([Required] [FromQuery] string username)
        {
            return HandleResult(await Mediator.Send(new SubscriptionCheckGetAll.Query{ Username = username}));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubscription([Required] [FromBody] SubscriptionCheckDto subscriptionCheck)
        {
            return HandleResult(await Mediator.Send(new SubscriptionCheckCreate.Command { SubscriptionCheck = subscriptionCheck}));
        }
    }
}