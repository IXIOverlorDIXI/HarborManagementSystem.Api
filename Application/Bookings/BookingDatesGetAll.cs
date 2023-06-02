using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Bookings
{
    public class BookingDatesGetAll
    {
        public class Query : IRequest<Result<List<BookingDates>>>
        {
            public BookingsFilter Filter { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<BookingDates>>>
        {
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;


            public Handler(IMapper mapper, IUserAccessor userAccessor, DataContext context)
            {
                _mapper = mapper;
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<List<BookingDates>>> Handle(Query request,
                CancellationToken cancellationToken)
            {

                var bookings = await _context.Bookings
                    .Where(x => x.EndDate > DateTime.Now 
                                && request.Filter.BerthId.Equals(x.BerthId)
                                && request.Filter.HarborId.Equals(x.Berth.HarborId)
                                && request.Filter.ShipId.Equals(x.ShipId))
                    .Include(x => x.BookingCheck)
                    .ToListAsync(cancellationToken);

                var bookingToView = new List<Booking>();
                var bookingToRemove = new List<Booking>();
                
                foreach (var booking in bookings)
                {
                    if (booking.StartDate <= DateTime.Now && booking.BookingCheck == null)
                    {
                        bookingToRemove.Add(booking);
                    }
                    else
                    {
                        bookingToView.Add(booking);
                    }
                }

                if (bookingToRemove.Any())
                {
                    _context.Bookings.RemoveRange(bookingToRemove);
                    
                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        return Result<List<BookingDates>>.Failure("Fail, the bookings does not exparied.");
                    }
                }

                var dates = bookingToView.Select(x => new BookingDates
                {
                    StartDate = x.StartDate,
                    EndDate = x.EndDate
                }).ToList();

                return Result<List<BookingDates>>
                    .Success(dates);
            }
        }
    }
}