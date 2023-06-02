using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Ships;
using Application.Ships.Photos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class ShipController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllShips()
        {
            return HandleResult(await Mediator.Send(new ShipGetAll.Query { }));
        }
        
        [HttpGet("byId")]
        public async Task<IActionResult> GetShip([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new ShipGet.Query { Id = id}));
        }

        [HttpPost]
        public async Task<IActionResult> CreateShip([Required] [FromBody] ShipDataDto ship)
        {
            return HandleResult(await Mediator.Send(new ShipCreate.Command { Ship = ship }));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateShip([Required] [FromBody] ShipDataDto ship)
        {
            return HandleResult(await Mediator.Send(new ShipUpdate.Command { Ship = ship }));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteShip([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new ShipDelete.Command { Id = id }));
        }

        [HttpGet("photo")]
        public async Task<IActionResult> GetShipPhoto([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new ShipPhotoGet.Query { Id = id }));
        }

        [HttpPost("photo")]
        public async Task<IActionResult> UploadShipPhoto([Required] [FromBody] ShipPhotoDataDto file)
        {
            return HandleResult(await Mediator.Send(new ShipPhotoUpload.Command { File = file }));
        }

        [HttpPut("photo")]
        public async Task<IActionResult> ChangeShipPhoto([Required] [FromBody] ShipPhotoDataDto file)
        {
            return HandleResult(await Mediator.Send(new ShipPhotoChange.Command { File = file }));
        }

        [HttpDelete("photo")]
        public async Task<IActionResult> DeleteShipPhoto([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new ShipPhotoDelete.Command { Id = id }));
        }
    }
}