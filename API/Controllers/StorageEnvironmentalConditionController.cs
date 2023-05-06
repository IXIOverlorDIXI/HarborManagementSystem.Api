using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.DTOs;
using Application.EnvironmentalCondition;
using Application.StorageEnvironmentalCondition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class StorageEnvironmentalConditionController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllStorageEnvironmentalConditions([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new StorageEnvironmentalConditionGetAll.Query { Id = id }));
        }

        //[AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateStorageEnvironmentalCondition([Required] [FromBody] StorageEnvironmentalConditionDto storageEnvironmentalCondition)
        {
            return HandleResult(await Mediator.Send(new StorageEnvironmentalConditionCreate.Command { StorageEnvironmentalCondition = storageEnvironmentalCondition }));
        }
    }
}