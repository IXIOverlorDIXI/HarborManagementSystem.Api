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
    public class BookingUpdate
    {
        public class Command : IRequest<Result<BookingDataDto>>
        {
            public BookingDataDto Booking { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<BookingDataDto>>
        {
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;

            public Handler(IMapper mapper,
                IUserAccessor userAccessor,
                DataContext context)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<BookingDataDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request.Booking.StartDate < DateTime.Now || request.Booking.StartDate >= request.Booking.EndDate)
                {
                    return Result<BookingDataDto>.Failure("This time diapason is wrong.");
                }
                
                if (_context.Bookings
                    .Any(x => 
                        !x.BerthId.Equals(request.Booking.BerthId)
                        && ( (x.EndDate > request.Booking.StartDate && x.EndDate < request.Booking.EndDate)
                             || (x.StartDate > request.Booking.StartDate && x.StartDate < request.Booking.EndDate)
                             || (x.StartDate < request.Booking.StartDate && x.StartDate > request.Booking.EndDate))))
                    
                {
                    return Result<BookingDataDto>.Failure("This time for berth has already taken.");
                }
                
                if (_context.Bookings
                    .Any(x => 
                        !x.ShipId.Equals(request.Booking.ShipId)
                        && ( (x.EndDate > request.Booking.StartDate && x.EndDate < request.Booking.EndDate)
                             || (x.StartDate > request.Booking.StartDate && x.StartDate < request.Booking.EndDate)
                             || (x.StartDate < request.Booking.StartDate && x.StartDate > request.Booking.EndDate))))
                    
                {
                    return Result<BookingDataDto>.Failure("This time for ship has already taken.");
                }

                var booking = await _context.Bookings
                    .Include(x => x.AdditionalServices)
                    .Include(x => x.BookingCheck)
                    .FirstOrDefaultAsync(x =>
                        x.Id.Equals(request.Booking.Id),
                    cancellationToken);

                if (booking == null)
                {
                    return Result<BookingDataDto>.Failure("Fail, this booking does not exist.");
                }
                
                if (booking.BookingCheck != null)
                {
                    return Result<BookingDataDto>.Failure("Fail, this booking is already payed.");
                }
                
                bool result;
                
                if (booking.StartDate <= DateTime.Now)
                {
                    _context.Bookings.Remove(booking);
                    
                    result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        return Result<BookingDataDto>.Failure("Fail, the booking does not exparied.");
                    }
                    
                    return Result<BookingDataDto>.Failure("Fail, this booking is exparied.");
                }

                var additionalServices = new List<AdditionalService>();

                foreach (var service in request.Booking.AdditionalServices)
                {
                    if (!_context.Services.Any(x => x.Id.Equals(service.Id)))
                    {
                        return Result<BookingDataDto>.Failure(String.Concat("Fail, service: \"", service.DisplayName, "\" does not exist."));
                    }

                    additionalServices.Add(new AdditionalService()
                    {
                        Id = Guid.NewGuid(),
                        ServiceId = service.Id,
                        BookingId = booking.Id
                    });
                }
                
                var additionalServicesToAdd = additionalServices
                    .Where(x => !booking.AdditionalServices
                        .Any(y => y.ServiceId.Equals(x.ServiceId)))
                    .ToList();

                var additionalServicesToRemove = booking.AdditionalServices
                    .Where(x => !additionalServices.Any(y => y.ServiceId.Equals(x.ServiceId)))
                    .ToList();

                _context.AdditionalServices.RemoveRange(additionalServicesToRemove);
                await _context.AdditionalServices.AddRangeAsync(additionalServicesToAdd, cancellationToken);

                result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<BookingDataDto>.Failure("Failed to create the booking.");
                }

                return Result<BookingDataDto>.Success(_mapper.Map<BookingDataDto>(booking));
            }
        }
    }
}