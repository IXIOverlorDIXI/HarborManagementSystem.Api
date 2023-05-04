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
    public class BookingForBerthGetAll
    {
        public class Query : IRequest<Result<List<BookingPreviewDataDto>>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<BookingPreviewDataDto>>>
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

            public async Task<Result<List<BookingPreviewDataDto>>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                if (!_context.Berths.Any(x => x.Id.Equals(request.Id)))
                {
                    return Result<List<BookingPreviewDataDto>>.Failure("Fail, berth does not exist.");
                }

                var user = await _context.Users
                    .Where(x => x.UserName.Equals(_userAccessor.GetUsername()))
                    .Include(x => x.Ships)
                    .Include(x => x.Harbors)
                    .ThenInclude(x => x.Berths)
                    .ThenInclude(x => x.Bookings)
                    .ThenInclude(x => x.AdditionalServices)
                    .ThenInclude(x => x.Service)
                    .Include(x => x.Ships)
                    .Include(x => x.Harbors)
                    .ThenInclude(x => x.Berths)
                    .ThenInclude(x => x.Bookings)
                    .ThenInclude(x => x.BookingCheck)
                    .FirstOrDefaultAsync(cancellationToken);

                var bookings = user.Harbors
                    .SelectMany(x => x.Berths.Where(x => x.Id.Equals(request.Id))
                        .SelectMany(y => y.Bookings))
                    .ToList();

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
                        return Result<List<BookingPreviewDataDto>>.Failure("Fail, the bookings does not exparied.");
                    }
                }

                return Result<List<BookingPreviewDataDto>>
                    .Success(_mapper.Map<List<BookingPreviewDataDto>>(bookingToView));
            }
        }
    }
}