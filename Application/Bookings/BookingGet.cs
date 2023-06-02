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
    public class BookingGet
    {
        public class Query : IRequest<Result<BookingEditDataDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<BookingEditDataDto>>
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

            public async Task<Result<BookingEditDataDto>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                var booking = await _context.Bookings
                    .Where(x => x.Id.Equals(request.Id))
                    .Include(x => x.BookingCheck)
                    .Include(x => x.AdditionalServices)
                    .ThenInclude(x => x.Service)
                    .Include(x => x.Berth)
                    .ThenInclude(x => x.Harbor)
                    .FirstOrDefaultAsync(cancellationToken);

                if (booking == null)
                {
                    return Result<BookingEditDataDto>.Failure("Fail, the booking does not exist.");
                }

                if (booking.StartDate <= DateTime.Now && booking.BookingCheck == null)
                {
                    _context.Bookings.Remove(booking);
                    
                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        return Result<BookingEditDataDto>.Failure("Fail, the booking does not exparied.");
                    }
                    
                    return Result<BookingEditDataDto>.Failure("Fail, the booking has been exparied.");
                }

                var bookingDto = _mapper.Map<BookingEditDataDto>(booking);
                bookingDto.AdditionalServices = new List<ServicePreviewDto>();

                foreach (var service in booking.AdditionalServices)
                {
                    bookingDto.AdditionalServices.Add(_mapper.Map<ServicePreviewDto>(service));
                }

                return Result<BookingEditDataDto>
                    .Success(bookingDto);
            }
        }
    }
}