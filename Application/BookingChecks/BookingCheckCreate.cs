using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.BookingChecks
{
    public class BookingCheckCreate
    {
        public class Command : IRequest<Result<BookingCheckDataDto>>
        {
            public BookingCheckDataDto BookingCheck { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<BookingCheckDataDto>>
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

            public async Task<Result<BookingCheckDataDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!_context.Bookings.Any(x => x.Id.Equals(request.BookingCheck.BookingId)))
                {
                    return Result<BookingCheckDataDto>.Failure("Fail, the booking does not exist.");
                }

                var booking = await _context.Bookings
                    .Include(x => x.BookingCheck)
                    .Include(x => x.AdditionalServices)
                    .ThenInclude(x => x.Service)
                    .Include(x => x.Berth)
                    .FirstOrDefaultAsync(x =>
                            x.Id.Equals(request.BookingCheck.BookingId),
                        cancellationToken);

                if (booking.BookingCheck != null)
                {
                    return Result<BookingCheckDataDto>.Failure("Fail, the booking check has already exist.");
                }

                if (booking.AdditionalServices.Sum(x => x.Service.Price) + booking.Berth.Price
                    * (booking.EndDate - booking.StartDate).Days != request.BookingCheck.TotalCost)
                {
                    return Result<BookingCheckDataDto>.Failure("Fail, the booking check total sum is wrong.");
                }

                var bookingCheck = _mapper.Map<Domain.Entities.BookingCheck>(request.BookingCheck);
                
                bookingCheck.Id = Guid.NewGuid();

                await _context.BookingChecks.AddAsync(bookingCheck, cancellationToken);
                
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<BookingCheckDataDto>.Failure("Failed to create the booking check.");
                }
                
                return Result<BookingCheckDataDto>
                    .Success(_mapper.Map<BookingCheckDataDto>(bookingCheck));
            }
        }
    }
}