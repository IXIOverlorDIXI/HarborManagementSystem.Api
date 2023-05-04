using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Bookings
{
    public class BookingDelete
    {
        public class Command : IRequest<Result<BookingDataDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<BookingDataDto>>
        {
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            private readonly IBlobManagerService _blobManagerService;

            public Handler(IMapper mapper,
                IUserAccessor userAccessor,
                DataContext context, IBlobManagerService blobManagerService)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;
                _blobManagerService = blobManagerService;
            }

            public async Task<Result<BookingDataDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var bookings = await _context.Bookings
                    .ToListAsync(cancellationToken);

                if (!bookings.Any(x => x.Id.Equals(request.Id)))
                {
                    return Result<BookingDataDto>.Failure("This bookings does not exists.");
                }

                var booking = await _context.Bookings
                    .Include(x => x.BookingCheck)
                    .FirstOrDefaultAsync(
                        x => x.Id.Equals(request.Id),
                        cancellationToken);

                if (booking.BookingCheck != null)
                {
                    return Result<BookingDataDto>.Failure("Failed to remove booking from database, it was payed.");
                }
                
                _context.Bookings.Remove(booking);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<BookingDataDto>.Failure("Failed to remove booking from database.");
                }

                return Result<BookingDataDto>.Success(_mapper.Map<BookingDataDto>(booking));
            }
        }
    }
}