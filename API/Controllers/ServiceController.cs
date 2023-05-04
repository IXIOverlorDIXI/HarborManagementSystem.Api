using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class ServiceController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllServices([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new ServiceGetAll.Query { Id = id }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateService([Required] [FromBody] ServiceDto service)
        {
            return HandleResult(await Mediator.Send(new ServiceCreate.Command { Service = service }));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateService([Required] [FromBody] ServiceDto service)
        {
            return HandleResult(await Mediator.Send(new ServiceUpdate.Command { Service = service }));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteService([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new ServiceDelete.Command { Id = id }));
        }
    }
}