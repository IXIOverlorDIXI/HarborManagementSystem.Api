using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.Berths;
using Application.Berths.Photos;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class BerthController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllBerths([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new BerthGetAll.Query { Id = id}));
        }
        
        [AllowAnonymous]
        [HttpGet("byId")]
        public async Task<IActionResult> GetBerth([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new BerthGet.Query { Id = id}));
        }

        [AllowAnonymous]
        [HttpPost("suitableBerths")]
        public async Task<IActionResult> GetAllSuitableBerths([Required] [FromBody] SuitableBerthSearchModel suitableBerthSearchModel)
        {
            return HandleResult(await Mediator.Send(new BerthGetAllSuitable.Query { SuitableBerthSearchModel = suitableBerthSearchModel }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBerth([Required] [FromBody] BerthDataDto berth)
        {
            return HandleResult(await Mediator.Send(new BerthCreate.Command { Berth = berth }));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBerth([Required] [FromBody] BerthDataDto berth)
        {
            return HandleResult(await Mediator.Send(new BerthUpdate.Command { Berth = berth }));
        }
        
        [HttpPut("turnOffOnTheBerth")]
        public async Task<IActionResult> UpdateBerth([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new BerthTurnOfOn.Command { Id = id }));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBerth([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new BerthDelete.Command { Id = id }));
        }
        
        [AllowAnonymous]
        [HttpGet("photos")]
        public async Task<IActionResult> GetBerthPhotos([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new BerthPhotosGetAll.Query { Id = id }));
        }
        
        [HttpPost("photos")]
        public async Task<IActionResult> UploadBerthPhoto([Required] [FromBody] BerthPhotoDataDto file)
        {
            return HandleResult(await Mediator.Send(new BerthPhotoUpload.Command { File = file }));
        }
        
        [HttpDelete("photos")]
        public async Task<IActionResult> DeleteBerthPhoto([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new BerthPhotoDelete.Command { Id = id }));
        }
    }
}