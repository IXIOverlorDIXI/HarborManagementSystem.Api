using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class SubscriptionController : BaseApiController
    {
        [HttpGet("currentSubscription")]
        public async Task<IActionResult> GetSubscription([Required] [FromQuery] string username)
        {
            return HandleResult(await Mediator.Send(new SubscriptionGet.Query{ Username = username}));
        }
        
        [AllowAnonymous]
        [HttpGet("allSubscriptions")]
        public async Task<IActionResult> GetAllSubscriptions([FromQuery] string username = "")
        {
            return HandleResult(await Mediator.Send(new SubscriptionGetAll.Query{ Username = username ?? ""}));
        }

        [HttpPost("changeSubscription")]
        public async Task<IActionResult> ChangeSubscription([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new SubscriptionChange.Command { Id = id}));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSubscription([Required] [FromBody] SubscriptionDto subscription)
        {
            return HandleResult(await Mediator.Send(new SubscriptionCreate.Command { Subscription = subscription}));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateSubscription([Required] [FromBody] SubscriptionDto subscription)
        {
            return HandleResult(await Mediator.Send(new SubscriptionUpdate.Command { Subscription = subscription }));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> DeleteSubscription([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new SubscriptionDelete.Command { Id = id}));
        }
    }
}