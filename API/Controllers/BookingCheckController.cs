using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.BookingChecks;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class BookingCheckController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllBookingChecks([Required] [FromQuery] string username)
        {
            return HandleResult(await Mediator.Send(new BookingCheckGetAll.Query { UserName = username }));
        }

        [HttpPut]
        public async Task<IActionResult> CreateBookingCheck([Required] [FromBody] BookingCheckDataDto bookingCheck)
        {
            return HandleResult(await Mediator.Send(new BookingCheckCreate.Command { BookingCheck = bookingCheck }));
        }
    }
}