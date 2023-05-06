using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.DTOs;
using Application.EnvironmentalCondition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class EnvironmentalConditionController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllEnvironmentalConditions([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new EnvironmentalConditionGetAll.Query { Id = id }));
        }

        //[AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateEnvironmentalCondition([Required] [FromBody] EnvironmentalConditionDto environmentalCondition)
        {
            return HandleResult(await Mediator.Send(new EnvironmentalConditionCreate.Command { EnvironmentalCondition = environmentalCondition }));
        }
    }
}