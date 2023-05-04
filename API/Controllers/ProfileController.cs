using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Profiles;
using Application.Profiles.Photos;
using Application.Profiles.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class ProfileController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetProfile([Required] [FromQuery] string username)
        {
            return HandleResult(await Mediator.Send(new ProfileGet.Query { Username = username }));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([Required] [FromBody] ProfileDto profile)
        {
            return HandleResult(await Mediator.Send(new ProfileUpdate.Command { Profile = profile }));
        }

        [HttpGet("photo")]
        public async Task<IActionResult> GetPhoto([Required] [FromQuery] string username)
        {
            return HandleResult(await Mediator.Send(new ProfilePhotoGet.Query { Username = username }));
        }

        [HttpPost("photo")]
        public async Task<IActionResult> UploadPhoto([Required] [FromBody] ProfilePhotoDataDto profilePhoto)
        {
            return HandleResult(await Mediator.Send(new ProfilePhotoUpload.Command { ProfilePhoto = profilePhoto }));
        }

        [HttpPut("photo")]
        public async Task<IActionResult> ChangePhoto([Required] [FromBody] ProfilePhotoDataDto profilePhoto)
        {
            return HandleResult(await Mediator.Send(new ProfilePhotoChange.Command { ProfilePhoto = profilePhoto }));
        }

        [HttpDelete("photo")]
        public async Task<IActionResult> DeletePhoto()
        {
            return HandleResult(await Mediator.Send(new ProfilePhotoDelete.Command { }));
        }
        
        [HttpGet("settings")]
        public async Task<IActionResult> GetSettings([Required] [FromQuery] string username)
        {
            return HandleResult(await Mediator.Send(new ProfileSettingsGet.Query { Username = username }));
        }

        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSettings([Required] [FromBody] SettingsDto settings)
        {
            return HandleResult(await Mediator.Send(new ProfileSettingsUpdate.Command { Settings = settings }));
        }
    }
}