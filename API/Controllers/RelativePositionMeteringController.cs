using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.DTOs;
using Application.RelativePositionMetering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class RelativePositionMeteringController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllRelativePositionMeterings([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new RelativePositionMeteringGetAll.Query { Id = id }));
        }

        //[AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateRelativePositionMetering([Required] [FromBody] RelativePositionMeteringDto relativePositionMetering)
        {
            return HandleResult(await Mediator.Send(new RelativePositionMeteringCreate.Command { RelativePositionMetering = relativePositionMetering }));
        }
    }
}