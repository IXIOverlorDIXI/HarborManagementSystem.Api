using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
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

                if ((await _context.Bookings
                        .FirstOrDefaultAsync(x =>
                                x.Id.Equals(request.BookingCheck.BookingId),
                            cancellationToken)).BookingCheck != null)
                {
                    return Result<BookingCheckDataDto>.Failure("Fail, the booking check has already exist.");
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