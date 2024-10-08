﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.Berths;
using Application.Bookings;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class BookingController : BaseApiController
    {
        [HttpGet("byId")]
        public async Task<IActionResult> GetBooking([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new BookingGet.Query { Id = id}));
        }
        
        [HttpGet("ownBookings")]
        public async Task<IActionResult> GetAllOwnBookings([Required] [FromQuery] string username)
        {
            return HandleResult(await Mediator.Send(new OwnBookingGetAll.Query { Username = username}));
        }
        
        [HttpGet("ownBookingsForShip")]
        public async Task<IActionResult> GetAllOwnBookingsForShip([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new OwnBookingForShipGetAll.Query { Id = id}));
        }
        
        [HttpGet("bookingsForHarbor")]
        public async Task<IActionResult> GetAllBookingsForHarbor([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new BookingForHarborGetAll.Query { Id = id}));
        }
        
        [HttpGet("bookingsForBerth")]
        public async Task<IActionResult> GetAllBookingsForBerth([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new BookingForBerthGetAll.Query { Id = id}));
        }
        
        [HttpGet("bookingDataForCheck")]
        public async Task<IActionResult> GetBookingDataForCheck([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new BookingGetDataForCheck.Query { Id = id}));
        }
        
        [HttpPost("getReservedDates")]
        public async Task<IActionResult> GetBookingsDates([Required] [FromBody] BookingsFilter filter)
        {
            return HandleResult(await Mediator.Send(new BookingDatesGetAll.Query { Filter = filter}));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([Required] [FromBody] BookingDataDto booking)
        {
            return HandleResult(await Mediator.Send(new BookingCreate.Command { Booking = booking }));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBooking([Required] [FromBody] BookingDataDto booking)
        {
            return HandleResult(await Mediator.Send(new BookingUpdate.Command { Booking = booking }));
        }
        
        [HttpDelete]
        public async Task<IActionResult> DeleteBooking([Required] [FromQuery] Guid id)
        {
            return HandleResult(await Mediator.Send(new BookingDelete.Command { Id = id }));
        }
    }
}