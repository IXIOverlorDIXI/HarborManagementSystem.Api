using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class ReviewController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet("byBerth")]
        public async Task<IActionResult> GetAllReviewsByBerth([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new ReviewByBerthGetAll.Query { Id = id }));
        }
        
        [AllowAnonymous]
        [HttpGet("byHarbor")]
        public async Task<IActionResult> GetAllReviewsByHarbor([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new ReviewByHarborGetAll.Query { Id = id }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([Required] [FromBody] ReviewDataDto review)
        {
            return HandleResult(await Mediator.Send(new ReviewCreate.Command { Review = review }));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateReview([Required] [FromBody] ReviewDataDto review)
        {
            return HandleResult(await Mediator.Send(new ReviewUpdate.Command { Review = review }));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteReview([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new ReviewDelete.Command { Id = id }));
        }
    }
}