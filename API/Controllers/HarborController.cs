using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Harbors;
using Application.Harbors.Documents;
using Application.Harbors.Photos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class HarborController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllHarbors()
        {
            return HandleResult(await Mediator.Send(new HarborGetAll.Query {}));
        }
        
        [AllowAnonymous]
        [HttpGet("byId")]
        public async Task<IActionResult> GetHarbor([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new HarborGet.Query {Id = id}));
        }
        
        [HttpGet("ownHarbors")]
        public async Task<IActionResult> GetAllOwnHarbors([Required] [FromQuery] string userName)
        {
            return HandleResult(await Mediator.Send(new HarborGetAllOwn.Query { UserName = userName}));
        }
        
        [AllowAnonymous]
        [HttpPost("suitableHarbors")]
        public async Task<IActionResult> GetAllSuitableHarbors([Required] [FromBody] ShipTypeDto shipType)
        {
            return HandleResult(await Mediator.Send(new HarborGetAllSuitable.Query { ShipType = shipType }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateHarbor([Required] [FromBody] HarborDataDto harbor)
        {
            return HandleResult(await Mediator.Send(new HarborCreate.Command { Harbor = harbor }));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateHarbor([Required] [FromBody] HarborDataDto harbor)
        {
            return HandleResult(await Mediator.Send(new HarborUpdate.Command { Harbor = harbor }));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteHarbor([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new HarborDelete.Command { Id = id }));
        }

        [AllowAnonymous]
        [HttpGet("photos")]
        public async Task<IActionResult> GetHarborPhotos([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new HarborPhotosGetAll.Query { Id = id }));
        }

        [HttpPost("photos")]
        public async Task<IActionResult> UploadHarborPhoto([Required] [FromBody] HarborPhotoDataDto file)
        {
            return HandleResult(await Mediator.Send(new HarborPhotoUpload.Command { File = file }));
        }

        [HttpDelete("photos")]
        public async Task<IActionResult> DeleteHarborPhoto([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new HarborPhotoDelete.Command { Id = id }));
        }
        
        [AllowAnonymous]
        [HttpGet("documents")]
        public async Task<IActionResult> GetHarborDocuments([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new HarborDocumentGetAll.Query { Id = id }));
        }

        [HttpPost("documents")]
        public async Task<IActionResult> UploadHarborDocument([Required] [FromBody] HarborDocumentDataDto file)
        {
            return HandleResult(await Mediator.Send(new HarborDocumentUpload.Command { File = file }));
        }

        [HttpDelete("documents")]
        public async Task<IActionResult> DeleteHarborDocument([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new HarborDocumentDelete.Command { Id = id }));
        }
    }
}