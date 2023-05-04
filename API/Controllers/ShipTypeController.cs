using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.DTOs;
using Application.ShipTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class ShipTypeController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllShipTypes()
        {
            return HandleResult(await Mediator.Send(new ShipTypeGetAll.Query{}));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateShipType([Required] [FromBody] ShipTypeDto shipType)
        {
            return HandleResult(await Mediator.Send(new ShipTypeCreate.Command { ShipType = shipType}));
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateShipType([Required] [FromBody] ShipTypeDto shipType)
        {
            return HandleResult(await Mediator.Send(new ShipTypeUpdate.Command { ShipType = shipType}));
        }
        
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> DeleteShipType([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new ShipTypeDelete.Command { Id = id}));
        }
    }
}